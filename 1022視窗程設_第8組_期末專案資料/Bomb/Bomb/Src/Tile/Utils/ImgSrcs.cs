using Bomb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Bomb.Scene
{
    class ImgSrcs : List<ImageSource>
    {
        public ImgSrcs(IImgSrcsLoader loader)
            : base()
        {
            this.AddRange(loader.getImageSources());


        }
        public ImgSrcs()
            : base()
        {
            
        }

        public ObjectAnimationUsingKeyFrames getFrameAnimation(int startFrame, int endFrame, TimeSpan duration, double loop, Image aniTarget, EventHandler completeCallback)
        {

           
            
            ObjectAnimationUsingKeyFrames frameAni = new ObjectAnimationUsingKeyFrames();
            
            frameAni.FillBehavior = FillBehavior.Stop;
            frameAni.Duration = new Duration(duration);
            frameAni.SpeedRatio = 1;
            frameAni.BeginTime = TimeSpan.FromSeconds(0);
            int totalFrame = (endFrame - startFrame)+1;
            
            for (int i = 0; i < (endFrame - startFrame) + 1; i++)
            {
                frameAni.KeyFrames.Add(new DiscreteObjectKeyFrame
                {
                    KeyTime = new TimeSpan(0, 0, 0, 0, (int)((duration.TotalMilliseconds / totalFrame) * (i + 1))),
                    Value = this[startFrame + i]


                });
            }
           
            Storyboard.SetTarget(frameAni, aniTarget);
            Storyboard.SetTargetProperty(frameAni, new PropertyPath("Source"));
            
            
            if (completeCallback != null)
            {
                frameAni.Completed += completeCallback;
            }
            if (loop == 0)
            {
                frameAni.RepeatBehavior = RepeatBehavior.Forever;
            }
            else
            {
                frameAni.RepeatBehavior = new RepeatBehavior(loop);
                
            }


            return frameAni;
        }

        

        
       
    }
}
