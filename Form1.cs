//#define MyDebug

using CrabShooter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

namespace CrabShooter
{
    public partial class CrabShooter : Form
    {
        const int FrameNum = 8;
        const int SplatNum = 7;

        bool splat = false;

        int gameFrame = 0;
        int deadCrabTime = 0;

        int hits = 0;
        int misses = 0;
        int totalShots = 0;
        double averageHits = 0;

#if MyDebug
        int cursX = 0;
        int cursY = 0;
#endif

        Crab crab;
        Sign sign;
        DeadCrab deadCrab;
        DeadCrab2 deadCrab2;
        DeadCrab3 deadCrab3;
        ScoreFrame scoreFrame;
        Random rnd = new Random();
        DirectHitSign directHit;

        public CrabShooter()
        {
            InitializeComponent();

            //Create scope site
            Bitmap b = new Bitmap(Resources.GunSight);
            this.Cursor = CustomCursor.CreateCursor(b, b.Height / 2, b.Width / 2);

            scoreFrame = new ScoreFrame() { Left = 200, Top = 1 };
            sign = new Sign() { Left = 560, Top = 230 };
            crab = new Crab() { Left = 330, Top = 450 };
            deadCrab = new DeadCrab();
            deadCrab2 = new DeadCrab2();
            deadCrab3 = new DeadCrab3();
            directHit = new DirectHitSign();
        }

        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            if(gameFrame >= FrameNum)
            {
                UpdateCrab();
                gameFrame = 0;
            }
            if (splat)
            {
                if(deadCrabTime >= SplatNum)
                {
                    splat = false;
                    deadCrabTime = 0;
                    UpdateCrab();
                }
                deadCrabTime++;
            }

            gameFrame++;
            this.Refresh();
        }

        private void UpdateCrab()
        {
            crab.Update(
                rnd.Next(Resources.Crab2.Width, this.Width - Resources.Crab2.Width),
                rnd.Next(this.Height / 2, this.Height - Resources.Crab2.Height * 2)
                );
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics dc = e.Graphics;

            if (splat == true)
            {
                //deadCrab.DrawImage(dc);
                deadCrab2.DrawImage(dc);
                deadCrab3.DrawImage(dc);
                directHit.DrawImage(dc);
            }
            else
            {
                crab.DrawImage(dc);
            }
            

            sign.DrawImage(dc);
            scoreFrame.DrawImage(dc);

#if MyDebug
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
            Font font = new System.Drawing.Font("Stencil", 12, FontStyle.Regular);
            TextRenderer.DrawText(dc, "X=" + cursX.ToString() + ":" + "Y=" + cursY.ToString(), font, new Rectangle(0, 0, 120, 20),
                SystemColors.ControlText, flags);
#endif

            //Put scores on the screen
            TextFormatFlags flags = TextFormatFlags.Left;
            Font font = new System.Drawing.Font("Stencil", 22, FontStyle.Bold);
            TextRenderer.DrawText(e.Graphics, totalShots.ToString(), font, new Rectangle(370, 26, 120, 50), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, hits.ToString(), font, new Rectangle(370, 55, 120, 50), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, misses.ToString(), font, new Rectangle(370, 87, 120, 50), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, averageHits.ToString("F0") + "%", font, new Rectangle(370, 117, 120, 50), SystemColors.ControlText, flags);

            base.OnPaint(e);
        }

        private void CrabShooter_MouseMove(object sender, MouseEventArgs e)
        {
#if MyDebug
            cursX = e.X;
            cursY = e.Y;
#endif
            this.Refresh();
        }

        private void CrabShooter_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 605 && e.X < 705 && e.Y > 260 && e.Y < 280)     //Start Hot Spot
            {
                timerGameLoop.Start();
            }  
            else if(e.X > 605 && e.X < 685 && e.Y > 290 && e.Y < 307)     //Stop Hot Spot
            {
                timerGameLoop.Stop();
            }
            else if (e.X > 605 && e.X < 700 && e.Y > 312 && e.Y < 333)     //Reset Hot Spot
            {
                timerGameLoop.Stop();
                totalShots = 0;
                hits = 0;
                misses = 0;
                averageHits = 0;
            }
            else if (e.X > 605 && e.X < 685 && e.Y > 340 && e.Y < 358)     //Quit Hot Spot
            {
                FireGun();
                timerGameLoop.Stop();
                Task.Delay(500).Wait();
                Form.ActiveForm.Close();
            }
            else
            {
                if(crab.Hit(e.X, e.Y))
                {
                    splat = true;

                    directHit.Left = 230;
                    directHit.Top = 400;
                    
                    //DeadCrab();
                    //DeadCrab2();
                    //DeadCrab3();

                    hits++;
                }
                else
                {
                    misses++;
                }
            
                totalShots = hits + misses;
                averageHits = ((double)hits / (double)totalShots) * 100.0;

            }

            FireGun();
        }

        private void FireGun()
        {
            //Fire off the shotgun
            SoundPlayer gunShot = new SoundPlayer(Resources.Shotgun);
            gunShot.Play();
        }

        //private void DeadCrab()
        //{
        //    deadCrab.Left = crab.Left - Resources.DeadCrab2.Width / 11;
        //    deadCrab.Top = crab.Top - Resources.DeadCrab2.Height / 1;
            
        //}

        //private void DeadCrab2()
        //{
        //    //Task.Delay(200).Wait();
        //    deadCrab2.Left = crab.Left - Resources.DeadCrab3.Width / 2;
        //    deadCrab2.Top = crab.Top - Resources.DeadCrab3.Height / 2;
        //}

        //private void DeadCrab3()
        //{
        //    deadCrab3.Left = crab.Left - Resources.DeadCrab4.Width / 1;
        //    deadCrab3.Top = crab.Top - Resources.DeadCrab4.Height / -4;
        //}

    }
}
