#region Header
/**
 * JsonMapper.cs
 *   JSON to .Net object and object to JSON conversions.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;


namespace LitJson
{
    internal struct PropertyMetadata
    {
        public MemberInfo Info;
        public bool IsField;
        public Type Type;
    }


    internal struct ArrayMetadata
    {
        private Type element_type;
        private bool is_array;
        private bool is_list;


        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set { element_type = value; }
        }

        public bool IsArray
        {
            get { return is_array; }
            set { is_array = value; }
        }

        public bool IsList
        {
            get { return is_list; }
            set { is_list = value; }
        }
    }


    internal struct ObjectMetadata
    {
        private Type element_type;
        private bool is_dictionary;

        private IDictionary<string, PropertyMetadata> properties;


        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set { element_type = value; }
        }

        public bool IsDictionary
        {
            get { return is_dictionary; }
            set { is_dictionary = value; }
        }

        public IDictionary<string, PropertyMetadata> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
    }


    internal delegate void ExporterFunc(object obj, JsonWriter writer);
    public delegate void ExporterFunc<T>(T obj, JsonWriter writer);

    internal delegate object ImporterFunc(object input);
    public delegate TValue ImporterFunc<TJson, TValue>(TJson input);

    public delegate IJsonWrapper WrapperFactory();


    public class JsonMapper
    {
        #region Fields
        //JohnLIn:
        public static bool shouldWriteNullProperty=false;

        private int max_nesting_depth;

        private IFormatProvider datetime_format;

        private IDictionary<Type, ExporterFunc> base_exporters_table;
        private IDictionary<Type, ExporterFunc> custom_exporters_table;

        private IDictionary<Type,
                IDictionary<Type, ImporterFunc>> base_importers_table;
        private IDictionary<Type,
                IDictionary<Type, ImporterFunc>> custom_importers_table;

        private IDictionary<Type, ArrayMetadata> array_metadata;
        private readonly object array_metadata_lock = new Object();

        private IDictionary<Type,
                IDictionary<Type, MethodInfo>> conv_ops;
        private readonly object conv_ops_lock = new Object();

        private IDictionary<Type, ObjectMetadata> object_metadata;
        private readonly object object_metadata_lock = new Object();

        private IDictionary<Type,
                IList<PropertyMetadata>> type_properties;
        private readonly object type_properties_lock = new Object();

        private JsonWriter static_writer;
        private readonly object static_writer_lock = new Object();
        #endregion


        #region Constructors
        public JsonMapper()
        {
            max_nesting_depth = 100;

            array_metadata = new Dictionary<Type, ArrayMetadata>();
            conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
            object_metadata = new Dictionary<Type, ObjectMetadata>();
            type_properties = new Dictionary<Type,
                            IList<PropertyMetadata>>();

            static_writer = new JsonWriter();

            datetime_format = DateTimeFormatInfo.InvariantInfo;

            base_exporters_table = new Dictionary<Type, ExporterFunc>();
            custom_exporters_table = new Dictionary<Type, ExporterFunc>();

            base_importers_table = new Dictionary<Type,
                                 IDictionary<Type, ImporterFunc>>();
            custom_importers_table = new Dictionary<Type,
                                   IDictionary<Type, ImporterFunc>>();

            RegisterBaseExporters();
            RegisterBaseImporters();
        }
        #endregion


        #region Private Methods
        private void AddArrayMetadata(Type type)
        {
            if (array_metadata.ContainsKey(type))
                return;

            ArrayMetadata data = new ArrayMetadata();

            data.IsArray = type.IsArray;

            if (type.GetInterface("System.Collections.IList") != null)
                data.IsList = true;

            foreach (PropertyInfo p_info in type.GetProperties())
            {
                if (p_info.Name != "Item")
                    continue;

                ParameterInfo[] parameters = p_info.GetIndexParameters();

                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                    data.ElementType = p_info.PropertyType;
            }

            lock (array_metadata_lock)
            {
                try
                {
                    array_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private void AddObjectMetadata(Type type)
        {
            if (object_metadata.ContainsKey(type))
                return;

            ObjectMetadata data = new ObjectMetadata();

            if (type.GetInterface("System.Collections.IDictionary") != null)
                data.IsDictionary = true;

            data.Properties = new Dictionary<string, PropertyMetadata>();

            foreach (PropertyInfo p_info in type.GetProperties())
            {
                if (p_info.Name == "Item")
                {
                    ParameterInfo[] parameters = p_info.GetIndexParameters();

                    if (parameters.Length != 1)
                        continue;

                    if (parameters[0].ParameterType == typeof(string))
                        data.ElementType = p_info.PropertyType;

                    continue;
                }

                PropertyMetadata p_data = new PropertyMetadata();
                p_data.Info = p_info;
                p_data.Type = p_info.PropertyType;

                data.Properties.Add(p_info.Name, p_data);
            }

            foreach (FieldInfo f_info in type.GetFields())
            {
                PropertyMetadata p_data = new PropertyMetadata();
                p_data.Info = f_info;
                p_data.IsField = true;
                p_data.Type = f_info.FieldType;

                data.Properties.Add(f_info.Name, p_data);
            }

            lock (object_metadata_lock)
            {
                try
                {
                    object_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private void AddTypeProperties(Type type)
        {
            if (type_properties.ContainsKey(type))
                return;

            IList<PropertyMetadata> props = new List<PropertyMetadata>();

            foreach (PropertyInfo p_info in type.GetProperties())
            {
                if (p_info.Name == "Item")
                    continue;

                PropertyMetadata p_data = new PropertyMetadata();
                p_data.Info = p_info;
                p_data.IsField = false;
                props.Add(p_data);
            }

            foreach (FieldInfo f_info in type.GetFields())
            {
                PropertyMetadata p_data = new PropertyMetadata();
                p_data.Info = f_info;
                p_data.IsField = true;

                props.Add(p_data);
            }

            lock (type_properties_lock)
            {
                try
                {
                    type_properties.Add(type, props);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private  MethodInfo GetConvOp(Type t1, Type t2)
        {
            lock (conv_ops_lock)
            {
                if (!conv_ops.ContainsKey(t1))
                    conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
            }

            if (conv_ops[t1].ContainsKey(t2))
                return conv_ops[t1][t2];

            MethodInfo op = t1.GetMethod(
                "op_Implicit", new Type[] { t2 });

            lock (conv_ops_lock)
            {
                try
                {
                    conv_ops[t1].Add(t2, op);
                }
                catch (ArgumentException)
                {
                    return conv_ops[t1][t2];
                }
            }

            return op;
        }

        private  object ReadValue(Type inst_type, JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd)
                return null;

            if (reader.Token == JsonToken.Null)
            {

                if (!inst_type.IsClass)
                    throw new JsonException(String.Format(
                            "Can't assign null to an instance of type {0}",
                            inst_type));

                return null;
            }

            if (reader.Token == JsonToken.Double ||
                reader.Token == JsonToken.Int ||
                reader.Token == JsonToken.Long ||
                reader.Token == JsonToken.String ||
                reader.Token == JsonToken.Boolean)
            {

                Type json_type = reader.Value.GetType();

                

                //JohnLin: primitive type
                if (inst_type.IsPrimitive && json_type.IsPrimitive)
                {
                    //they can do implicit cast so just assign json value into instance directly
                    return reader.Value;
                }

                //reference type
                if (inst_type.IsAssignableFrom(json_type))
                    return reader.Value;

                //JohnLin: we should try to use custom importer first
                // If there's a custom importer that fits, use it
                if (custom_importers_table.ContainsKey(json_type))
                {
                    foreach(KeyValuePair<Type,ImporterFunc> entry in custom_importers_table[json_type])
                    {
                        if (entry.Key.IsAssignableFrom(inst_type))
                        {
                            ImporterFunc importer = entry.Value;

                            return importer(reader.Value);
                        }
                    }
                }

                // Maybe there's a base importer that works
                if (base_importers_table.ContainsKey(json_type) &&
                    base_importers_table[json_type].ContainsKey(
                        inst_type))
                {

                    ImporterFunc importer =
                        base_importers_table[json_type][inst_type];

                    return importer(reader.Value);
                }

                // Maybe it's an enum
                if (inst_type.IsEnum)
                    return Enum.ToObject(inst_type, reader.Value);

                // Try using an implicit conversion operator
                MethodInfo conv_op = GetConvOp(inst_type, json_type);

                if (conv_op != null)
                    return conv_op.Invoke(null,
                                           new object[] { reader.Value });

                

                // No luck
                throw new JsonException(String.Format(
                        "Can't assign value '{0}' (type {1}) to type {2}",
                        reader.Value, json_type, inst_type));
            }

            object instance = null;

            if (reader.Token == JsonToken.ArrayStart)
            {

                //JohnLin: we should try to use custom importer first
                // If there's a custom importer that fits, use it
                if (custom_importers_table.ContainsKey(typeof(JsonReader)))
                {
                    foreach (KeyValuePair<Type, ImporterFunc> entry in custom_importers_table[typeof(JsonReader)])
                    {
                        if (entry.Key.IsAssignableFrom(inst_type))
                        {
                            ImporterFunc importer = entry.Value;

                            return importer(reader);
                        }
                    }
                }
                
                AddArrayMetadata(inst_type);
                ArrayMetadata t_data = array_metadata[inst_type];

                if (!t_data.IsArray && !t_data.IsList)
                    throw new JsonException(String.Format(
                            "Type {0} can't act as an array",
                            inst_type));

                IList list;
                Type elem_type;

                if (!t_data.IsArray)
                {
                    list = (IList)Activator.CreateInstance(inst_type);
                    elem_type = t_data.ElementType;
                }
                else
                {
                    list = new ArrayList();
                    elem_type = inst_type.GetElementType();
                }

                while (true)
                {
                    object item = ReadValue(elem_type, reader);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    list.Add(item);
                }

                if (t_data.IsArray)
                {
                    int n = list.Count;
                    instance = Array.CreateInstance(elem_type, n);

                    for (int i = 0; i < n; i++)
                        ((Array)instance).SetValue(list[i], i);
                }
                else
                    instance = list;

            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                //JohnLin: we should try to use custom importer first
                // If there's a custom importer that fits, use it
                if (custom_importers_table.ContainsKey(typeof(JsonReader)))
                {
                    foreach (KeyValuePair<Type, ImporterFunc> entry in custom_importers_table[typeof(JsonReader)])
                    {
                        if (entry.Key.IsAssignableFrom(inst_type))
                        {
                            ImporterFunc importer = entry.Value;
                            
                            return importer(reader);
                        }
                    }
                }
                AddObjectMetadata(inst_type);
                ObjectMetadata t_data = object_metadata[inst_type];

                instance = Activator.CreateInstance(inst_type);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;

                    string property = (string)reader.Value;

                    if (t_data.Properties.ContainsKey(property))
                    {
                        PropertyMetadata prop_data =
                            t_data.Properties[property];

                        if (prop_data.IsField)
                        {
                            ((FieldInfo)prop_data.Info).SetValue(
                                instance, ReadValue(prop_data.Type, reader));
                        }
                        else
                        {
                            PropertyInfo p_info =
                                (PropertyInfo)prop_data.Info;

                            if (p_info.CanWrite)
                                p_info.SetValue(
                                    instance,
                                    ReadValue(prop_data.Type, reader),
                                    null);
                            else
                                ReadValue(prop_data.Type, reader);
                        }

                    }
                    else
                    {
                        if (!t_data.IsDictionary)
                        {

                            if (!reader.SkipNonMembers)
                            {
                                throw new JsonException(String.Format(
                                        "The type {0} doesn't have the " +
                                        "property '{1}'",
                                        inst_type, property));
                            }
                            else
                            {
                                ReadSkip(reader);
                                continue;
                            }
                        }


                        ((IDictionary)instance).Add(
                          property, ReadValue(
                              t_data.ElementType, reader));
                        
                    }

                }

            }

            return instance;
        }

        private  IJsonWrapper ReadValue(WrapperFactory factory,
                                               JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd ||
                reader.Token == JsonToken.Null)
                return null;

            IJsonWrapper instance = factory();

            if (reader.Token == JsonToken.String)
            {
                instance.SetString((string)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Double)
            {
                instance.SetDouble((double)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Int)
            {
                instance.SetInt((int)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Long)
            {
                instance.SetLong((long)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Boolean)
            {
                instance.SetBoolean((bool)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.ArrayStart)
            {
                instance.SetJsonType(JsonType.Array);

                while (true)
                {
                    IJsonWrapper item = ReadValue(factory, reader);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    ((IList)instance).Add(item);
                }
            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                instance.SetJsonType(JsonType.Object);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;

                    string property = (string)reader.Value;

                    ((IDictionary)instance)[property] = ReadValue(
                        factory, reader);
                }

            }

            return instance;
        }

        private  void ReadSkip(JsonReader reader)
        {
            ToWrapper(
                delegate { return new JsonMockWrapper(); }, reader);
        }

        private  void RegisterBaseExporters()
        {
            base_exporters_table[typeof(byte)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((byte)obj));
                };

            base_exporters_table[typeof(char)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToString((char)obj));
                };

            base_exporters_table[typeof(DateTime)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToString((DateTime)obj,
                                                    datetime_format));
                };

            base_exporters_table[typeof(decimal)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write((decimal)obj);
                };

            base_exporters_table[typeof(sbyte)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((sbyte)obj));
                };

            base_exporters_table[typeof(short)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((short)obj));
                };

            base_exporters_table[typeof(ushort)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToInt32((ushort)obj));
                };

            base_exporters_table[typeof(uint)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write(Convert.ToUInt64((uint)obj));
                };

            base_exporters_table[typeof(ulong)] =
                delegate(object obj, JsonWriter writer)
                {
                    writer.Write((ulong)obj);
                };
        }

        private  void RegisterBaseImporters()
        {
            ImporterFunc importer;

            importer = delegate(object input)
            {
                return Convert.ToByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(byte), importer);

            importer = delegate(object input)
            {
                return Convert.ToUInt64((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(ulong), importer);

            importer = delegate(object input)
            {
                return Convert.ToSByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(sbyte), importer);

            importer = delegate(object input)
            {
                return Convert.ToInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(short), importer);

            importer = delegate(object input)
            {
                return Convert.ToUInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(ushort), importer);

            importer = delegate(object input)
            {
                return Convert.ToUInt32((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(uint), importer);

            importer = delegate(object input)
            {
                return Convert.ToSingle((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(float), importer);

            importer = delegate(object input)
            {
                return Convert.ToDouble((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(double), importer);

            importer = delegate(object input)
            {
                return Convert.ToDecimal((double)input);
            };
            RegisterImporter(base_importers_table, typeof(double),
                              typeof(decimal), importer);


            importer = delegate(object input)
            {
                return Convert.ToUInt32((long)input);
            };
            RegisterImporter(base_importers_table, typeof(long),
                              typeof(uint), importer);

            importer = delegate(object input)
            {
                return Convert.ToChar((string)input);
            };
            RegisterImporter(base_importers_table, typeof(string),
                              typeof(char), importer);

            importer = delegate(object input)
            {
                return Convert.ToDateTime((string)input, datetime_format);
            };
            RegisterImporter(base_importers_table, typeof(string),
                              typeof(DateTime), importer);
        }

        private  void RegisterImporter(
            IDictionary<Type, IDictionary<Type, ImporterFunc>> table,
            Type json_type, Type value_type, ImporterFunc importer)
        {
            if (!table.ContainsKey(json_type))
                table.Add(json_type, new Dictionary<Type, ImporterFunc>());

            table[json_type][value_type] = importer;
        }

        private  void WriteValue(object obj, JsonWriter writer,
                                        bool writer_is_private,
                                        int depth)
        {
            if (depth > max_nesting_depth)
                throw new JsonException(
                    String.Format("Max allowed object depth reached while " +
                                   "trying to export from type {0}",
                                   obj.GetType()));


            if (obj == null)
            {
                writer.Write(null);
                return;
            }

            if (obj is IJsonWrapper)
            {
                if (writer_is_private)
                    writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
                else
                    ((IJsonWrapper)obj).ToJson(writer);

                return;
            }

            


            if (obj is String)
            {

                writer.Write((string)obj);
                return;
            }

            if (obj is Double)
            {
                writer.Write((double)obj);
                return;
            }

            if (obj is Int32)
            {
                writer.Write((int)obj);
                return;
            }

            if (obj is Boolean)
            {
                writer.Write((bool)obj);
                return;
            }

            if (obj is Int64)
            {
                writer.Write((long)obj);
                return;
            }

            if (obj is Array)
            {
                
                writer.WriteArrayStart();

                foreach (object elem in (Array)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1);

                writer.WriteArrayEnd();

                return;
            }

            if (obj is IList)
            {
               
                writer.WriteArrayStart();
                foreach (object elem in (IList)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1);
                writer.WriteArrayEnd();

                return;
            }

            if (obj is IDictionary)
            {
               
                
                writer.WriteObjectStart();
                foreach (DictionaryEntry entry in (IDictionary)obj)
                {
                    if (!shouldWriteNullProperty && entry.Value == null)
                    {
                        continue;
                    }
                    writer.WritePropertyName((string)entry.Key);
                    WriteValue(entry.Value, writer, writer_is_private,
                                depth + 1);
                }
                writer.WriteObjectEnd();

                return;
            }

            Type obj_type = obj.GetType();


            //JohnLin: we shuld try to exprot with custom exporter first
            // See if there's a custom exporter for the object
            //if (custom_exporters_table.ContainsKey(obj_type))
            //{
            //    ExporterFunc exporter = custom_exporters_table[obj_type];
            //    exporter(obj, writer);

            //    return;
            //}
            foreach (KeyValuePair<Type, ExporterFunc> entry in custom_exporters_table)
            {
                if (entry.Key.IsAssignableFrom(obj_type))
                {
                    ExporterFunc exporter = entry.Value;
                    exporter(obj, writer);

                    return;
                }
            }

            // If not, maybe there's a base exporter
            if (base_exporters_table.ContainsKey(obj_type))
            {

                ExporterFunc exporter = base_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            //let's see if it's an enum
            if (obj is Enum)
            {
                Type e_type = Enum.GetUnderlyingType(obj_type);

                if (e_type == typeof(long)
                    || e_type == typeof(uint)
                    || e_type == typeof(ulong))
                    writer.Write((ulong)obj);
                else
                    writer.Write((int)obj);

                return;
            }

            // Okay, so it looks like the input should be exported as an
            // object

            if (!shouldWriteNullProperty && obj == null)
            {
                return;
            }

            AddTypeProperties(obj_type);
            IList<PropertyMetadata> props = type_properties[obj_type];

            writer.WriteObjectStart();
            foreach (PropertyMetadata p_data in props)
            {
                if (p_data.IsField)
                {
                    object fieldVal = ((FieldInfo)p_data.Info).GetValue(obj);
                    if (!shouldWriteNullProperty && fieldVal == null)
                    {
                        continue;
                    }
                    writer.WritePropertyName(p_data.Info.Name);
                    WriteValue(fieldVal,
                                writer, writer_is_private, depth + 1);
                }
                else
                {
                    PropertyInfo p_info = (PropertyInfo)p_data.Info;
                    object propertyVal = p_info.GetValue(obj, null);
                    if (!shouldWriteNullProperty && propertyVal == null)
                    {
                        continue;
                    }
                    if (p_info.CanRead)
                    {
                        writer.WritePropertyName(p_data.Info.Name);
                        WriteValue(propertyVal,
                                    writer, writer_is_private, depth + 1);
                    }
                }
            }
            writer.WriteObjectEnd();
        }
        #endregion


        public  string ToJson(object obj)
        {
            lock (static_writer_lock)
            {
                static_writer.Reset();

                WriteValue(obj, static_writer, true, 0);

                return static_writer.ToString();
            }
        }

        public  void ToJson(object obj, JsonWriter writer)
        {
            WriteValue(obj, writer, false, 0);
        }

        public  JsonData ToObject(JsonReader reader)
        {
            return (JsonData)ToWrapper(
                delegate { return new JsonData(this); }, reader);
        }

        public  JsonData ToObject(TextReader reader)
        {
            JsonReader json_reader = new JsonReader(reader);

            return (JsonData)ToWrapper(
                delegate { return new JsonData(this); }, json_reader);
        }

        public  JsonData ToObject(string json)
        {
            return (JsonData)ToWrapper(
                delegate { return new JsonData(this); }, json);
        }

        public  T ToObject<T>(JsonReader reader)
        {
            return (T)ReadValue(typeof(T), reader);
        }

        public  T ToObject<T>(TextReader reader)
        {
            JsonReader json_reader = new JsonReader(reader);

            return (T)ReadValue(typeof(T), json_reader);
        }

        public  T ToObject<T>(string json)
        {
            JsonReader reader = new JsonReader(json);
            return (T)ReadValue(typeof(T), reader);
        }

        public T ToGenericObject<T>(string json)
        {
            //JohnLin: this method is just same as "public  T ToObject<T>(string json)" method, but using different name so that we can find this method quicker with reflection
            //thus to avoid ambiguous exception 
            return ToObject<T>(json);
        }
        public T ToGenericObject<T>(JsonReader reader)
        {
            //JohnLin: this method is just same as "public  T ToObject<T>(JsonReader json)" method, but using different name so that we can find this method quicker with reflection
            //thus to avoid ambiguous exception 
            return ToObject<T>(reader);
        }
        

        public  IJsonWrapper ToWrapper(WrapperFactory factory,
                                              JsonReader reader)
        {
            return ReadValue(factory, reader);
        }

        public  IJsonWrapper ToWrapper(WrapperFactory factory,
                                              string json)
        {
            JsonReader reader = new JsonReader(json);

            return ReadValue(factory, reader);
        }

        public  void RegisterExporter<T>(ExporterFunc<T> exporter)
        {
            ExporterFunc exporter_wrapper =
                delegate(object obj, JsonWriter writer)
                {
                    exporter((T)obj, writer);
                };

            custom_exporters_table[typeof(T)] = exporter_wrapper;
        }

        public  void RegisterImporter<TJson, TValue>(
            ImporterFunc<TJson, TValue> importer)
        {
            ImporterFunc importer_wrapper =
                delegate(object input)
                {
                    return importer((TJson)input);
                };

            RegisterImporter(custom_importers_table, typeof(TJson),
                              typeof(TValue), importer_wrapper);
        }
        

        public  void UnregisterExporters()
        {
            custom_exporters_table.Clear();
        }

        public  void UnregisterImporters()
        {
            custom_importers_table.Clear();
        }
    }
}
