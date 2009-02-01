using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;

namespace InventIt.SiteSystem.Library
{
    public static class ImageVertexRounding
    {
        public static Image RoundedRectangle(Image imgPhoto, int Radius)
        {
            using (System.Drawing.Image imgin = imgPhoto)
            {
                System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(imgin.Width, imgin.Height);
                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.Clear(Color.Transparent);
                Brush brush = new System.Drawing.TextureBrush(imgin);

                FillRoundedRectangle(grPhoto, new Rectangle(1, 1, imgin.Width - 3, imgin.Height - 3), (Radius * 2), brush);

                grPhoto.Dispose();

                return (Image)bmPhoto;
            }
        }

        public static Image RoundedRectangle(Image imgPhoto, int Radius, int BorderWidth, string BorderColor)
        {
            using (System.Drawing.Image imgin = imgPhoto)
            {
                System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(imgin.Width, imgin.Height);
                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.Clear(Color.Transparent);
                Brush brush = new System.Drawing.TextureBrush(imgin);
                Pen tempPen = new Pen(GetColorFromARGB(BorderColor), BorderWidth);

                FillRoundedRectangle(grPhoto, new Rectangle(1, 1, imgin.Width - 3, imgin.Height - 3), (Radius * 2), brush);
                DrawRoundedRectangle(grPhoto, new Rectangle(1, 1, imgin.Width - 3, imgin.Height - 3), (Radius * 2), tempPen);

                grPhoto.Dispose();

                return (Image)bmPhoto;
            }
        }

        // http://forums.asp.net/p/942160/1128861.aspx
        private static void FillRoundedRectangle(Graphics grPhoto, Rectangle r, int d, Brush b)
        {
            Graphics tempPhoto = grPhoto;
            int tempInt = d / 2;

            // Create points that define polygon.
            Point p1 = new Point(tempInt, 1);
            Point p2 = new Point(r.Width - tempInt, 1);
            Point p3 = new Point(r.Width, tempInt);
            Point p4 = new Point(r.Width, r.Height - tempInt);
            Point p5 = new Point(r.Width - tempInt, r.Height);
            Point p6 = new Point(tempInt, r.Height);
            Point p7 = new Point(1, r.Height - tempInt);
            Point p8 = new Point(1, tempInt);
            Point[] points = {p1, p2, p3, p4, p5, p6, p7, p8};

            // Define fill mode.
            FillMode newFillMode = FillMode.Winding;

            tempPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            tempPhoto.FillPolygon(b, points, newFillMode);

            // anti alias distorts fill so remove it.
            System.Drawing.Drawing2D.SmoothingMode mode = grPhoto.SmoothingMode;

            grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            grPhoto.FillPie(b, r.X, r.Y, d, d, 180, 90);
            grPhoto.FillPie(b, r.X + r.Width - d, r.Y, d, d, 270, 90);
            grPhoto.FillPie(b, r.X, r.Y + r.Height - d, d, d, 90, 90);
            grPhoto.FillPie(b, r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            grPhoto.FillRectangle(b, r.X + d / 2, r.Y, r.Width - d, d / 2);
            grPhoto.FillRectangle(b, r.X, r.Y + d / 2, r.Width, r.Height - d);
            grPhoto.FillRectangle(b, r.X + d / 2, r.Y + r.Height - d / 2, r.Width - d, d / 2);
            grPhoto.SmoothingMode = mode;

            grPhoto = tempPhoto;
        }

        // http://forums.asp.net/p/942160/1128861.aspx
        public static void DrawRoundedRectangle(Graphics grPhoto, Rectangle r, int d, Pen p)
        {
            // anti alias distorts fill so remove it.
            System.Drawing.Drawing2D.SmoothingMode mode = grPhoto.SmoothingMode;

            grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            grPhoto.DrawArc(p, r.X, r.Y, d, d, 180, 90);
            grPhoto.DrawLine(p, r.X + d / 2, r.Y, r.X + r.Width - d / 2, r.Y);
            grPhoto.DrawArc(p, r.X + r.Width - d, r.Y, d, d, 270, 90);
            grPhoto.DrawLine(p, r.X, r.Y + d / 2, r.X, r.Y + r.Height - d / 2);
            grPhoto.DrawLine(p, r.X + r.Width, r.Y + d / 2, r.X + r.Width, r.Y + r.Height - d / 2);
            grPhoto.DrawLine(p, r.X + d / 2, r.Y + r.Height, r.X + r.Width - d / 2, r.Y + r.Height);
            grPhoto.DrawArc(p, r.X, r.Y + r.Height - d, d, d, 90, 90);
            grPhoto.DrawArc(p, r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            grPhoto.SmoothingMode = mode;
        }

        // http://dotnet-snippets.de/dns/c-hexstring-in-systemdrawingcolor-umwandeln-SID95.aspx
        public static Color GetColorFromARGB(string hexString)
        {
            int red = int.Parse(hexString.Substring(1, 2), NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(3, 2), NumberStyles.HexNumber);
            int blue = int.Parse(hexString.Substring(5, 2), NumberStyles.HexNumber);

            return Color.FromArgb(red, green, blue);
        }
    }
}
