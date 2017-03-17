using Rust_Interceptor.Data;
using Rust_Interceptor.Forms.Hooks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static Rust_Interceptor.Forms.Structs.WindowStruct;

namespace Rust_Interceptor.Forms
{
    public partial class Overlay : Form
    {
        //Parametros necesarios para simular el desplazamiento del Form
        private bool mouseDown;
        private Point lastLocation;
        //////////////////////////////////////////

        public bool working = false;
        private RustInterceptor sniffer;
        private Thread worker;
        private Process rustProcess;
        private readonly String rustProcessName = "RustClient";

        private System.Collections.Generic.HashSet<Entity> listaUsuarios = new HashSet<Entity>();
        private delegate void genericCallback();

        public Overlay()
        {
            InitializeComponent();
        }

        private void Overlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.working = false;
        }

        private void Overlay_Load(object sender, EventArgs e)
        {
            //this.MaximumSize = this.Size;

            this.labelIp.Click += new EventHandler(
                (object responsable, EventArgs evento) =>
                {
                    this.textBoxIp.Focus();
                });
            this.labelPuerto.Click += new EventHandler(
                (object responsable, EventArgs evento) =>
                {
                    this.textBoxPuerto.Focus();
                });
            this.buttonEmpezar.Click += new EventHandler(
                (object responsable, EventArgs evento) =>
                {
                    this.working = true;
                    sniffer = new RustInterceptor( this.textBoxIp.Text, Convert.ToInt32(this.textBoxPuerto.Text) );
                    sniffer.AddPacketsToFilter(Packet.Rust.Entities, Packet.Rust.EntityDestroy, Packet.Rust.EntityPosition);
                    sniffer.packetHandlerCallback = packetHandler;
                    this.prepareOverlay();
                });

        }

        //Sobreescribe WndProc para permitir que el evento de despalzar el Form siga siendo posible
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
        /// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC </param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        //Oculta todos los elementos para poder tener un overlayForm
        public void prepareOverlay()
        {
            foreach ( Control ctrl in this.Controls ) //Oculto todos los controles
            {
                ctrl.Visible = false;
            }
            this.labelPlayers.Visible = true;

            this.BackColor = Color.Black; //Seteamos el color de fondo a negro.
            this.TransparencyKey = Color.Black; //Indicamos que todo lo que se encuentre en negro dentro del formulario sea transparente.

            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);

            //Creo un Hilo que se encargara de hacer todo el curro
            worker = new Thread(
                () =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Thread.CurrentThread.Name = "OverlayWorkerThread";
                    while( rustProcess == null ) // Iteramos hasta que el proceso Rust se haya inicializado y lo capturamos
                    {
                        Console.WriteLine("Buscando proceso de Rust...");
                        searchRustProcess(out rustProcess);
                        Thread.Sleep(1 * 1000);
                    }
                    rustProcess.EnableRaisingEvents = true;
                    rustProcess.Exited += new EventHandler( //Ahora
                        (object sender, EventArgs e) =>
                        {
                            this.Cerrar();
                        });

                    new WindowHook(rustProcess.MainWindowHandle, this);

                    //sniffer.packetHandlerCallback = packetHandler;
                    sniffer.Start();
                    do
                    {
                        worldToScreen();
                        Thread.Sleep(50);
                    } while (working);

                });

            worker.Start();
        }

        //Se ocupara de mostrar en el overlay todo las entidades que nos interesa
        private void worldToScreen()
        {
            if (this.InvokeRequired) this.Invoke(new genericCallback(worldToScreen));
            else
            {
                Graphics g = this.CreateGraphics();
                g.Clear(Color.Black);
                Brush pincel = new SolidBrush(Color.Green);
                this.drawCrosshair(g, new Pen(pincel));

                this.labelPlayers.ResetText();
                /*
                lock (listaUsuarios)
                {
                    foreach (Entity entidad in listaUsuarios)
                    {
                        this.labelPlayers.Text += entidad.Data.basePlayer.name + " {" + entidad.Position + "} || HP -> " + entidad.Data.basePlayer.currentLife + "\n";
                    }
                }*/
                //e.Graphics.DrawString("Hello World!!!", fuente, pincel , 0, 0);
                /*
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle( 50, 50, 150, 150);
                graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
                graphics.DrawRectangle(System.Drawing.Pens.Red, rectangle);
                graphics.Clear(Color.Black);
                */
            }

        }

        private void packetHandler(Packet packet)
        {
            Entity entity;
            switch (packet.rustID)
            {
                case Packet.Rust.Entities:
                    ProtoBuf.Entity entityInfo;
                    uint num = Data.Entity.ParseEntity(packet, out entityInfo);
                    entity = Entity.CreateOrUpdate(num, entityInfo);
                    if (entity != null)
                    {
                        if (entity.IsPlayer)
                        {
                            if(!entity.IsLocalPlayer)  listaUsuarios.Add(entity);
                        }
                    }
                return;
                case Packet.Rust.EntityPosition:
                    List<Data.Entity.EntityUpdate> updates = Data.Entity.ParsePositions(packet);
                    List<Entity> entities = null;
                    if (updates.Count == 1)
                    {
                        entity = Entity.UpdatePosistion(updates[0]);
                        if (entity != null)
                        {
                            if (entity.IsPlayer)
                            {
                                if (!entity.IsLocalPlayer)
                                {
                                    (entities = new List<Entity>()).Add(entity);
                                }
                            }
                           
                        }
                    }
                    else if (updates.Count > 1)
                    {
                        entities = Entity.UpdatePositions(updates);
                    }
                    if (entities != null)
                    {
                        this.resetText(this.labelPlayers);
                        entities.ForEach(entidad =>
                        {
                            this.appendText( this.labelPlayers,entidad.Data.basePlayer.name + " {" + entidad.Position + "} || HP -> " + entidad.Data.basePlayer.currentLife + "\n" );
                        });
                    }
                 break;
            }
        }
        






        private void drawCrosshair(Graphics g, Pen lapiz)
        {
            lapiz.Width = 1;
            lapiz.Color = Color.Red;
            //linea horizontal
            Point middleMinorH = new Point((this.Width / 2) - 5, (this.Height / 2));
            Point middleMayorH = new Point((this.Width / 2) + 5, (this.Height / 2));
            g.DrawLine(lapiz, middleMinorH, middleMayorH);
            //linea vertical
            Point middleMinorV = new Point((this.Width / 2), (this.Height / 2) - 5);
            Point middleMayorV = new Point((this.Width / 2), (this.Height / 2) + 5);
            g.DrawLine(lapiz, middleMinorV, middleMayorV);
        }

        private void searchRustProcess(out Process proceso)
        {
            Process[] procesos = Process.GetProcessesByName(rustProcessName);
            proceso = null;
            if (procesos.Length > 0)
            {
                Console.WriteLine("Proceso encontrado");
                proceso = procesos[0];
            }
        }

        private delegate void resetTextCallback(Control elemento);
        private void resetText(Control elemento)
        {
            if (this.InvokeRequired) this.Invoke(new resetTextCallback(resetText), elemento);
            else
            {
                elemento.ResetText();
            }
        }
        private delegate void appendTextCallback(Control elemento,String cadena);
        private void appendText(Control elemento, String cadena)
        {
            if (this.InvokeRequired) this.Invoke(new appendTextCallback(appendText), elemento, cadena);
            else
            {
                elemento.Text += cadena;
            }
        }
        private delegate void resizeFormCallback(RECT rectangulo);
        public void resizeForm(RECT rectangulo)
        {
            if (this.InvokeRequired) this.Invoke(new resizeFormCallback(resizeForm),rectangulo);
            else
            {
                this.Size = rectangulo.Size;
                //Setting Window position;
                this.Top = rectangulo.Top;
                this.Left = rectangulo.Left;
            }
        }

        private void Cerrar()
        {
            if (this.InvokeRequired) this.Invoke(new genericCallback(Cerrar));
            else
            {
                this.Close();
                sniffer.Stop();
                Application.Exit();
            }
        }
    }
}
