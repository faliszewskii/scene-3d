namespace scene_3d
{
    public partial class Scene3DApp : Form
    {
        private Scene3D scene;
        private SceneDrawer sceneDrawer;
        private Bitmap canvas;

        public Scene3DApp()
        {
            InitializeComponent();
            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = canvas;
            scene = new Scene3D();
            sceneDrawer = new SceneDrawer(canvas);
            timer.Enabled = true;
        }

        private void loadOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadObj(new Point(pictureBox.Width / 2, pictureBox.Height / 2));
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
        }

        private void loadObj(Point position)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                scene.AddObj(openFileDialog.FileName, position);
            }
            sceneDrawer.drawScene(scene, canvas);
            pictureBox.Refresh();
        }

        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            loadObj(e.Location);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            scene.rotateObj();
            sceneDrawer.drawScene(scene, canvas);
            pictureBox.Refresh();
        }

        private void fovTrackBar_Scroll(object sender, EventArgs e)
        {
            sceneDrawer.Fov = fovTrackBar.Value;
        }
    }
}