using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinearOperators
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private float w, h, x0, y0, time;
        private Pen blackPen = new Pen(Brushes.Black);
        private Pen redPen = new Pen(Brushes.Red, 3f);
        private float timerTick = 1000 / 30f;
        private List<GeometryObject> gObjects;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            w = panel1.Width;
            h = panel1.Height;
            x0 = w / 2;
            y0 = h / 2;

            InitObjects();

            timer = new Timer();
            timer.Interval = (int)timerTick;
            timer.Tick += PanelPaint;
            timer.Enabled = true;
        }

        private void InitObjects()
        {
            gObjects = new List<GeometryObject>();

            var o1 = new GeometryObject(new PointF(0, 0), new PointF(50, 50), "first", new Pen(Brushes.Green, 1));
            gObjects.Add(o1);
            o1.AddVertex(new PointF(0, 60));

            var o2 = new GeometryObject(new PointF(0, 0), new PointF(50, 50), "second", new Pen(Brushes.Red, 3));
            gObjects.Add(o2);
            o2.ChangeVertex(0, new PointF(0, -40));
            o2.ChangeVertex(1, new PointF(60, 30));
            o2.AddVertex(new PointF(30, 70));
            o2.AddVertex(new PointF(0, 50));
            o2.AddVertex(new PointF(-30, 70));
            o2.AddVertex(new PointF(-60, 30));
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            w = panel1.Width;
            h = panel1.Height;
            x0 = w / 2;
            y0 = h / 2;
        }

        #region old & useless
        private float getRealX(float x) { return x0 + x; }
        private float getRealY(float y) { return y0 - y; }

        private float getX(float x) { return x - x0; }
        private float getY(float y) { return y0 - y; }
        #endregion

        private PointF getCoordinatesOnScreen(PointF p)
        {
            p.X = x0 + p.X;
            p.Y = y0 - p.Y;

            return p;
        }

        private void DrawPoint(Graphics g, PointF p, float pDiameter = 10, Brush brush = null)
        {
            if (brush == null) brush = Brushes.Blue;

            g.FillEllipse(brush, (x0 + p.X) - pDiameter / 2, (y0 - p.Y) - pDiameter / 2, pDiameter, pDiameter);
        }

        private void DrawObject(Graphics g, GeometryObject o, Pen pen = null)
        {
            if (pen == null) pen = redPen;

            g.DrawPolygon(pen, o.GetScreenCoordinates(getCoordinatesOnScreen));
        }

        private void PanelPaint(object sender, EventArgs e)
        {
            time += timerTick;

            var bmp = new Bitmap((int)w, (int)h);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawLine(blackPen, getCoordinatesOnScreen(new PointF(-w/2, 0)), getCoordinatesOnScreen(new PointF(w/2, 0)));
            g.DrawLine(blackPen, getCoordinatesOnScreen(new PointF(0, -h/2)), getCoordinatesOnScreen(new PointF(0, h/2)));

            var old_point = new PointF((float)Math.Sin((time - timerTick) / 10000 * 2 * Math.PI) * 150, -(float)Math.Cos((time - timerTick) / 10000 * 2 * Math.PI) * 100);
            var o1_old_center = gObjects[0].GetCenter();

            var rel1 = new PointF((float)Math.Sin(time / 10000 * 2 * Math.PI) * 150, -(float)Math.Cos(time / 10000 * 2 * Math.PI) * 100);
            var offset1 = new PointF(rel1.X - old_point.X, rel1.Y - old_point.Y);
            gObjects[0].StandartTransform(-(float)(timerTick / 5000 * 2 * Math.PI), offset1, rel1);

            var rel2 = gObjects[0].GetCenter();
            var offset2 = new PointF((rel2.X - o1_old_center.X), (rel2.Y - o1_old_center.Y));
            gObjects[1].StandartTransform((float)(timerTick/500 * 2*Math.PI), offset2, rel2);


            // отрисовка объектов
            for (int i = 0; i < gObjects.Count; i++)
                DrawObject(g, gObjects[i], gObjects[i].Pen);
            
            // отрисовка вершин
            for (int i = 0; i < gObjects.Count; i++)
                for (int j = 0; j < gObjects[i].VertexCount; j++)
                    DrawPoint(g, gObjects[i].Vertexes[j]);

            // отрисовка точек
            DrawPoint(g, rel1, pDiameter: 8f, Brushes.Black);
            DrawPoint(g, rel2, pDiameter: 8f, Brushes.Black);
            DrawPoint(g, gObjects[1].GetCenter(), pDiameter: 8f, Brushes.Black);

            panel1.BackgroundImage = bmp;
        }
    }
}
