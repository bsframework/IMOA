using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMOAWinClient
{
    public class FormDockTemplate : NativeWindow   
         {  
             #region 私有字段   
            /// <summary>    
             /// 父级窗口实例    
             /// </summary>    
             private Form parentForm = null;   
             /// <summary>    
             /// 窗口实例的启动信息    
            /// </summary>    
             private FormStartInfo m_oFormStartInfo = null;   
             /// <summary>    
             /// 当前窗口可以停靠的方式    
             /// </summary>    
             private Enu_FormDockStyle m_iDockStyle = Enu_FormDockStyle.None;   
             /// <summary>    
             /// 窗口停靠检测的定时器    
             /// </summary>    
             private Timer m_tmrHideWindow = null;   
             /// <summary>    
             /// 自动感知的矩形区域    
             /// </summary>    
            private Rectangle m_rcLeft = Rectangle.Empty;   
             private Rectangle m_rcTop = Rectangle.Empty;   
             private Rectangle m_rcRight = Rectangle.Empty;   
             private Rectangle m_rcBottom = Rectangle.Empty;   
             /// <summary>    
             /// 感知区域的容差值，也就是当鼠标移动进入距边缘几个象素时进行自动捕获    
             /// </summary>    
             private int m_iSensitiveAreaTolerantPixel = 4;  
            #endregion  
      
             #region 字段属性   
             /// <summary>    
            /// 当前窗口的鼠标位置    
             /// </summary>    
             Point CurrentMousePos   
             {   
                 get  
                 {   
                     //获取当前鼠标的屏幕坐标               
                     User32.POINT ptMousePos = new User32.POINT();   
                     User32.GetCursorPos(ref ptMousePos);   
                    return new Point(ptMousePos.X, ptMousePos.Y);   
                 }   
             }   
       
             Rectangle FormTitleRect   
             {   
                 get { return new Rectangle(parentForm.Location, new Size(parentForm.Width, parentForm.Height - parentForm.ClientRectangle.Height)); }   
             }   
       
             /// <summary>    
             /// 感应区域的容差设置，距离屏幕边缘多少象素时，开始自动感知    
             /// </summary>    
             public int TolerantPixel   
             {   
                 get { return m_iSensitiveAreaTolerantPixel; }   
                 set { m_iSensitiveAreaTolerantPixel = value; }   
             }  
             #endregion  
      
             #region 构造函数   
            /// <summary>    
             /// 构造函数    
             /// </summary>    
             /// <param name="frmParent">父窗口对象</param>    
             public FormDockTemplate(Form frmParent)   
                 : this(frmParent, 4)   
             {   
             }   
       
             /// <summary>    
             /// 构造函数    
             /// </summary>    
             /// <param name="frmParent">父窗口对象</param>    
            /// <param name="iTolerantPixel">自动感知容差象素（当Mouse距离屏幕边缘多少象素时自动感知）</param>    
             public FormDockTemplate(Form frmParent, int iTolerantPixel)   
            {   
                m_iSensitiveAreaTolerantPixel = iTolerantPixel;   
                parentForm = frmParent;   
                parentForm.HandleCreated += new EventHandler(parentForm_HandleCreated);   
                parentForm.HandleDestroyed += new EventHandler(parentForm_HandleDestroyed);   
                 parentForm.Load += new EventHandler(parentForm_Load);   
                 parentForm.Move += new EventHandler(parentForm_Move);               
                parentForm.Resize += new EventHandler(parentForm_Resize);   
      
                //初始化窗体的启动信息：如上次关闭时窗体的大小及位置    
                InitialFormStartInfo();   
             }   
      
         /// <summary>   
         /// 初始化窗体启动信息，通过反序列化完成   
         /// </summary>   
         void InitialFormStartInfo()   
         {   
             try  
             {   
                 m_oFormStartInfo = new FormStartInfo(parentForm);   
                FormStartInfo.Deserialize(ref m_oFormStartInfo);                   
            }   
             catch  
             {   
                 m_oFormStartInfo.FormLocation = parentForm.Location;   
                 m_oFormStartInfo.FormSize = new Size(parentForm.Width, parentForm.Height);   
             }   
         }  
         #endregion  
 
         #region 窗体事件处理   
         void parentForm_Load(object sender, EventArgs e)   
         {   
             //初始化感知区域    
             InitialDockArea();   
   
             //初始化时设置窗口大小及位置   
             parentForm.Location = m_oFormStartInfo.FormLocation;   
             parentForm.Size = m_oFormStartInfo.FormSize;   
   
             //定时器初始化    
             m_tmrHideWindow = new Timer();   
             m_tmrHideWindow.Interval = 100;   
             m_tmrHideWindow.Enabled = true;   
             m_tmrHideWindow.Tick += new EventHandler(m_tmrHideWindow_Tick);   
         }   
   
        void parentForm_Resize(object sender, EventArgs e)   
         {   
            m_oFormStartInfo.FormSize = parentForm.Size;   
        }   
   
         void parentForm_Move(object sender, EventArgs e)   
        {   
            //当左键按下时并且当前鼠标位置处于窗口标题栏区域内，则认为是合法窗口移动，启用自动感知功能    
             if (Control.MouseButtons == MouseButtons.Left && FormTitleRect.Contains(CurrentMousePos))   
             {   
                 SetFormDockPos();                   
             }   
         }   
           
         void parentForm_HandleDestroyed(object sender, EventArgs e)   
         {   
             //销毁定时器    
             m_tmrHideWindow.Enabled = false;   
            m_tmrHideWindow.Stop();   
             m_tmrHideWindow.Dispose();   
   
            //窗口关闭时，保存窗口的大小位置及停靠信息    
             if (m_iDockStyle == Enu_FormDockStyle.None)   
             {   
                 m_oFormStartInfo.FormLocation = parentForm.Location;   
                 m_oFormStartInfo.FormSize = parentForm.Size;   
             }   
             FormStartInfo.Serialize(m_oFormStartInfo);   
   
             //释放本类关联的窗口句柄    
             ReleaseHandle();   
         }   
   
         void parentForm_HandleCreated(object sender, EventArgs e)   
         {   
            AssignHandle(((Form)sender).Handle);   
         }   
   
         void m_tmrHideWindow_Tick(object sender, EventArgs e)   
         {   
            if (m_oFormStartInfo.DockStyle != Enu_FormDockStyle.None)   
            {   
                //为了提升显示效率，只有处于如下两种情况时，才需要重新显示窗体    
                 //1、窗体可见但鼠标已经移出窗体外    
                 //2、窗体不可见但鼠标已经移入窗体内    
                 bool bNeedReshow = (m_oFormStartInfo.FormVisible && IsMouseOutForm()) ||   
                     (!m_oFormStartInfo.FormVisible && !IsMouseOutForm());   
                 if (bNeedReshow)   
                     m_oFormStartInfo.ShowDockWindow(parentForm.Handle, !IsMouseOutForm());   
             }   
         }  
        #endregion  
  
         #region 私有函数   
         private void InitialDockArea()   
         {   
             //获取屏幕可用区域                
             User32.RECT rectWorkArea = new User32.RECT();   
            User32.SystemParametersInfo((uint)User32.Enu_SystemParametersInfo_Action.SPI_GETWORKAREA, 0, ref rectWorkArea, 0);   
             Rectangle rcWorkArea = new Rectangle(rectWorkArea.left, rectWorkArea.top, rectWorkArea.right - rectWorkArea.left, rectWorkArea.bottom - rectWorkArea.top);   
             Rectangle rcScreenArea = Screen.PrimaryScreen.Bounds;   
  
             //容差值，表示鼠标移动到边界若干象素里即可以自动感知停靠位置                
             m_rcLeft = new Rectangle(rcWorkArea.Left, rcWorkArea.Top, m_iSensitiveAreaTolerantPixel, rcWorkArea.Height);   
             m_rcTop = new Rectangle(rcWorkArea.Left, rcWorkArea.Top, rcWorkArea.Width, m_iSensitiveAreaTolerantPixel);   
             m_rcRight = new Rectangle(rcWorkArea.Width - rcWorkArea.Left - m_iSensitiveAreaTolerantPixel, rcWorkArea.Top, m_iSensitiveAreaTolerantPixel, rcWorkArea.Height);   
             m_rcBottom = new Rectangle(rcScreenArea.Left, rcScreenArea.Bottom - rcScreenArea.Top - m_iSensitiveAreaTolerantPixel, rcScreenArea.Width, m_iSensitiveAreaTolerantPixel);   
         }   
            
         /// <summary>    
         /// 鼠标按下时未放开的时候，设置窗体停靠时的位置    
         /// </summary>    
         void SetFormDockPos()   
         {   
             m_iDockStyle = Enu_FormDockStyle.None;   
   
             //根据不同的停靠方式来重置窗体位置    
            if (m_rcLeft.Contains(CurrentMousePos))   
             {   
                 parentForm.Location = m_rcLeft.Location;   
                 parentForm.Height = m_rcLeft.Height;   
   
                 m_iDockStyle = Enu_FormDockStyle.Left;   
             }   
             else if (m_rcTop.Contains(CurrentMousePos))   
             {   
                 parentForm.Location = new Point(parentForm.Location.X, m_rcTop.Top);   
   
                 m_iDockStyle = Enu_FormDockStyle.Top;   
             }   
             else if (m_rcRight.Contains(CurrentMousePos))   
            {   
                 parentForm.Location = new Point(m_rcRight.Right - parentForm.Width, m_rcRight.Top);   
                 parentForm.Height = m_rcRight.Height;   
   
                 m_iDockStyle = Enu_FormDockStyle.Right;   
             }   
             else if (m_rcBottom.Contains(CurrentMousePos))   
             {   
                parentForm.Location = new Point(parentForm.Location.X, m_rcBottom.Bottom - parentForm.Height);   
   
                 m_iDockStyle = Enu_FormDockStyle.Bottom;   
             }   
   
            m_oFormStartInfo.DockStyle = m_iDockStyle;   
             m_oFormStartInfo.FormLocation = parentForm.Location;               
         }   
   
         /// <summary>    
        /// 表明当前鼠标位置是否已经移出窗体外    
         /// </summary>    
         /// <returns></returns>    
         private bool IsMouseOutForm()   
         {   
             //获取当前鼠标的屏幕坐标               
             User32.POINT ptMousePos = new User32.POINT();   
             User32.GetCursorPos(ref ptMousePos);   
             Point ptClientCursor = new Point(ptMousePos.X, ptMousePos.Y);   
   
             User32.RECT rcFormClient = new User32.RECT();   
             User32.GetWindowRect(this.Handle, ref rcFormClient);   
             Rectangle rcFormBound = new Rectangle(rcFormClient.left, rcFormClient.top, rcFormClient.right - rcFormClient.left, rcFormClient.bottom - rcFormClient.top);   
             return !rcFormBound.Contains(ptClientCursor);   
         }  
         #endregion   
     }   
 }  

