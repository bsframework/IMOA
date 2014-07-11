using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMOAWinClient
{
    public enum Enu_FormDockStyle
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 3,
        Bottom = 4,
    }

    [Serializable]
    public class FormStartInfo
    {
        [NonSerialized]
        private Form m_frmDockWindow = null;
        private string m_strSerialFileName = string.Empty;
        private Size m_szFormSize = Size.Empty;
        private Point m_ptFormLocation = Point.Empty;
        private Enu_FormDockStyle m_iDockStyle = Enu_FormDockStyle.None;
        private bool m_bFormVisible = false;


        /// <summary>    
        /// 构造函数    
        /// </summary>    
        /// <param name="frmItem">停靠的窗体对象</param>    
        public FormStartInfo(Form frmItem)
        {
            try
            {
                m_frmDockWindow = frmItem;

                if (null == frmItem) m_strSerialFileName = "StartInfo.dat";
                else m_strSerialFileName = frmItem.Name + frmItem.Text + "_StartInfo.dat";
            }
            catch { }
        }

        /// <summary>    
        /// 窗体大小    
        /// </summary>    
        public Size FormSize
        {
            get { return m_szFormSize; }
            internal set { m_szFormSize = value; }
        }

        /// <summary>    
        /// 窗体位置坐标    
        /// </summary>    
        public Point FormLocation
        {
            get { return m_ptFormLocation; }
            internal set { m_ptFormLocation = value; }
        }

        /// <summary>    
        /// 停靠方式    
        /// </summary>    
        public Enu_FormDockStyle DockStyle
        {
            get { return m_iDockStyle; }
            internal set { m_iDockStyle = value; }
        }

        /// <summary>    
        /// 表示窗体是否自动隐藏    
        /// </summary>    
        public bool FormVisible
        {
            get { return m_bFormVisible; }
        }
        /// <summary>    
        /// 序列化此类的实例信息    
        /// </summary>    
        /// <param name="frmStartInfo"></param>    
        public static void Serialize(FormStartInfo frmStartInfo)
        {
            using (FileStream fs = new FileStream(frmStartInfo.m_strSerialFileName, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, frmStartInfo);
            }
        }

        /// <summary>    
        /// 反序列化此类的实例信息    
        /// </summary>    
        /// <param name="frmStartInfo"></param>    
        public static void Deserialize(ref FormStartInfo frmStartInfo)
        {
            FormStartInfo frmTemp = null;

            if (null == frmStartInfo) return;
            using (FileStream fs = new FileStream(frmStartInfo.m_strSerialFileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                frmTemp = (FormStartInfo)bf.Deserialize(fs);
                if (null != frmTemp) frmStartInfo = frmTemp;
            }
        }
        /// <summary>    
        /// 显示或隐藏停靠窗口    
        /// </summary>    
        public void ShowDockWindow(IntPtr hwnd, bool bVisible)
        {
            Point ptLocation = Point.Empty;
            Size szFormSize = Size.Empty;

            m_bFormVisible = bVisible;

            if (m_frmDockWindow == null) m_frmDockWindow = (Form)Control.FromHandle(hwnd);
            if (m_frmDockWindow == null) return;



            GetDockWindowClientRect(ref ptLocation, ref szFormSize, bVisible);

            m_frmDockWindow.TopMost = (m_iDockStyle != Enu_FormDockStyle.None);
            m_frmDockWindow.Location = ptLocation;
            m_frmDockWindow.Width = szFormSize.Width;
            m_frmDockWindow.Height = szFormSize.Height;
        }
        /// <summary>    
        /// 根据当前窗体的停靠方式来计算出当前窗体的大小及位置    
        /// </summary>    
        /// <param name="ptLocation">窗体位置</param>    
        /// <param name="szFormSize">窗体大小</param>    
        /// <param name="bDockWindowVisible">显示还是隐藏</param>    
        private void GetDockWindowClientRect(ref Point ptLocation, ref Size szFormSize, bool bDockWindowVisible)
        {
            int iTorrentPixel = 0;
            int iWindowTitleHeight = SystemInformation.CaptionHeight;


            //获取屏幕可用区域                
            User32.RECT rectWorkArea = new User32.RECT();
            User32.SystemParametersInfo((uint)User32.Enu_SystemParametersInfo_Action.SPI_GETWORKAREA, 0, ref rectWorkArea, 0);
            Rectangle rcWorkArea = new Rectangle(rectWorkArea.left, rectWorkArea.top, rectWorkArea.right - rectWorkArea.left, rectWorkArea.bottom - rectWorkArea.top);
            Rectangle rcScreenArea = Screen.PrimaryScreen.Bounds;

            if (m_ptFormLocation.X < 0) m_ptFormLocation.X = 0;
            if (m_ptFormLocation.Y < 0) m_ptFormLocation.Y = 0;

            if (!bDockWindowVisible)
            {
                switch (m_iDockStyle)
                {
                    case Enu_FormDockStyle.None:
                        ptLocation = m_ptFormLocation;
                        szFormSize = m_szFormSize;
                        break;
                    case Enu_FormDockStyle.Left:
                        ptLocation = new Point(m_ptFormLocation.X - m_szFormSize.Width + SystemInformation.FrameBorderSize.Width + iTorrentPixel, rcWorkArea.Top);
                        szFormSize = new Size(m_szFormSize.Width, rcWorkArea.Height);
                        break;
                    case Enu_FormDockStyle.Top:
                        ptLocation = new Point(m_ptFormLocation.X, rcWorkArea.Top - m_szFormSize.Height + SystemInformation.FrameBorderSize.Width + iTorrentPixel);
                        szFormSize = m_szFormSize;
                        break;
                    case Enu_FormDockStyle.Right:
                        ptLocation = new Point(rcWorkArea.Width - rcWorkArea.Left - SystemInformation.FrameBorderSize.Width - iTorrentPixel, rcWorkArea.Top);
                        szFormSize = new Size(m_szFormSize.Width, rcWorkArea.Height);
                        break;
                    case Enu_FormDockStyle.Bottom:
                        ptLocation = new Point(m_ptFormLocation.X, rcScreenArea.Bottom - rcScreenArea.Top - SystemInformation.FrameBorderSize.Width - iTorrentPixel);
                        szFormSize = m_szFormSize;
                        break;
                    default:
                        ptLocation = m_ptFormLocation;
                        szFormSize = m_szFormSize;
                        break;
                }
            }
            else
            {
                ptLocation = m_ptFormLocation;
                szFormSize = m_szFormSize;
            }
        }
    }
}

