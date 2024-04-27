using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathTracer
{
  public partial class MainWindow : Form
  {
    System.Windows.Forms.Timer updateUI = new System.Windows.Forms.Timer();
    Stopwatch sw = new Stopwatch();
    Renderer r = null;
    const int bitmapWidth = 160;
    Bitmap bmp;


    CancellationTokenSource tokenSource;
    Task renderTask;

    public float lightRadius = 80;
    public float lightIntensity = 20;

    public float emissionSpectrum = 1.0f;

    
    public MainWindow()
    {
      InitializeComponent();
      pbxRender.Image = bmp;

      updateUI.Tick += UpdateUI_Tick;
      updateUI.Interval = 200;
      updateUI.Start();
    }

    public float returnLightRadius()
    {
      return lightRadius;
    }

    public float returnLightIntensity()
    {
      return lightIntensity;
    }

    private void UpdateUI_Tick(object sender, EventArgs e)
    {
      if (bmp != null)
        r?.CopyBitmap(bmp);
      lblSPP.Text = $"SPP: {r?.SPP ?? 0}";
      lblTime.Text = $"Time: {sw.Elapsed.ToString()}";
      pbxRender.Invalidate();
      pbxRender.Update();
    }

    private void btnDoit_Click(object sender, EventArgs e)
    {
      if (renderTask != null && !renderTask.IsCompleted && !renderTask.IsCanceled && !renderTask.IsFaulted)
      {
        tokenSource.Cancel();
        renderTask.Wait();
        tokenSource.Dispose();
        sw.Stop();

        try
        {
          //lightRadius = float.Parse(txtLightRadius.Text);
          // lightIntensity = float.Parse(txtLightIntensity.Text);
        }
        catch (FormatException)
        {
          // Handle invalid input (e.g., show an error message)
          MessageBox.Show("Invalid values entered. Please enter numbers.");
          return;
        }

        // ... existing code ...

        // Update light properties with user-entered values
        //light.Radius = lightRadius;
        //light.Intensity = lightIntensity;
        return;
      }

      tokenSource = new CancellationTokenSource();

      Scene s = Scene.CornellBox(lightIntensity, lightRadius, emissionSpectrum);

      bmp = new Bitmap(bitmapWidth, (int)Math.Round(bitmapWidth / s.AspectRatio), PixelFormat.Format24bppRgb);
      pbxRender.Image = bmp;

      using (Graphics grD = Graphics.FromImage(bmp))
      {
        grD.Clear(Color.Black);
      }

      r = new Renderer(bmp);

      var token = tokenSource.Token;
      sw.Restart();
      renderTask = Task.Run(() => r.Render(s, token), token);

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void label2_Click(object sender, EventArgs e)
    {

    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      var txt = sender as TextBox;
      // check if text is not empty and we can parse it to a float
      if (txt.Text != "" && float.TryParse(txt.Text, out float result))
      {
        lightRadius = float.Parse(txt.Text);
      }

    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {
      var txt = sender as TextBox;
      if (txt.Text != "" && float.TryParse(txt.Text, out float result))
      {
        lightIntensity = float.Parse(txt.Text);
      }

    }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt.Text != "" && float.TryParse(txt.Text, out float result))
            {
                emissionSpectrum = float.Parse(txt.Text);
            }
        }
    }
}
