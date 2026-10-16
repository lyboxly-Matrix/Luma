// SPDX-License-Identifier: GPL-2.0-or-later
using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices;


static class Theme {
 public static Color Base=Color.FromArgb(10,16,29),Surface=Color.FromArgb(15,23,40),Line=Color.FromArgb(37,49,71),Text=Color.FromArgb(226,235,248),Muted=Color.FromArgb(137,161,191),Cyan=Color.FromArgb(13,190,218),Purple=Color.FromArgb(139,85,246);
 public static System.Drawing.Drawing2D.GraphicsPath Round(RectangleF r,float radius){var p=new System.Drawing.Drawing2D.GraphicsPath();float d=radius*2;p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
}
class GlowButton : Button {
 public bool Accent,RoundPlay;bool hover,down;
 public GlowButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;BackColor=Theme.Surface;ForeColor=Theme.Text;Cursor=Cursors.Hand;}
 protected override void OnMouseEnter(EventArgs e){hover=true;Invalidate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){hover=false;down=false;Invalidate();base.OnMouseLeave(e);}protected override void OnMouseDown(MouseEventArgs e){down=true;Invalidate();base.OnMouseDown(e);}protected override void OnMouseUp(MouseEventArgs e){down=false;Invalidate();base.OnMouseUp(e);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.Clear(BackColor);g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;var r=new RectangleF(1,1,Width-3,Height-3);using(var path=Theme.Round(r,RoundPlay?(Height-3)/2:Math.Min(15,Height/2-2))){if(Accent){using(var b=new System.Drawing.Drawing2D.LinearGradientBrush(r,down?Theme.Purple:Theme.Cyan,Theme.Purple,35f))g.FillPath(b,path);}else using(var b=new SolidBrush(down?Color.FromArgb(33,47,72):hover?Color.FromArgb(25,38,59):Theme.Surface))g.FillPath(b,path);using(var pen=new Pen(Accent?Color.FromArgb(93,144,233):hover?Theme.Cyan:Theme.Line,1))g.DrawPath(pen,path);}TextRenderer.DrawText(g,Text,Font,ClientRectangle,Enabled?ForeColor:Theme.Muted,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.NoPadding);if(Focused&&ShowFocusCues)ControlPaint.DrawFocusRectangle(g,new Rectangle(5,5,Width-10,Height-10),Theme.Cyan,BackColor);}
}
class DarkMenuColors : ProfessionalColorTable {public override Color ToolStripDropDownBackground{get{return Theme.Surface;}}public override Color MenuBorder{get{return Theme.Line;}}public override Color MenuItemBorder{get{return Theme.Line;}}public override Color MenuItemSelected{get{return Color.FromArgb(28,48,70);}}public override Color CheckBackground{get{return Theme.Surface;}}public override Color CheckSelectedBackground{get{return Theme.Surface;}}}
class DarkCombo : ComboBox {
 public DarkCombo(){DrawMode=DrawMode.OwnerDrawFixed;FlatStyle=FlatStyle.Flat;BackColor=Theme.Surface;ForeColor=Theme.Text;ItemHeight=24;}
 protected override void OnDrawItem(DrawItemEventArgs e){if(e.Index<0)return;bool selected=(e.State&DrawItemState.Selected)!=0;using(var b=new SolidBrush(selected?Color.FromArgb(32,54,79):Theme.Surface))e.Graphics.FillRectangle(b,e.Bounds);TextRenderer.DrawText(e.Graphics,GetItemText(Items[e.Index]),Font,new Rectangle(e.Bounds.X+6,e.Bounds.Y,e.Bounds.Width-8,e.Bounds.Height),Theme.Text,TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);}
}
class SurfacePanel : Panel {
 public SurfacePanel(){DoubleBuffered=true;BackColor=Theme.Base;}
 protected override void OnPaintBackground(PaintEventArgs e){e.Graphics.Clear(BackColor);e.Graphics.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;var r=new RectangleF(1,1,Width-3,Height-3);if(r.Width<=0||r.Height<=0)return;using(var p=Theme.Round(r,14))using(var b=new System.Drawing.Drawing2D.LinearGradientBrush(r,Color.FromArgb(19,30,48),Color.FromArgb(21,17,39),15f))using(var pen=new Pen(Theme.Line)){e.Graphics.FillPath(b,p);e.Graphics.DrawPath(pen,p);}}
}

class SlimSeek : Control {
 public int Maximum=10000; int val;
 public int Value{get{return val;}set{val=Math.Max(0,Math.Min(Maximum,value));Invalidate();}}
 public SlimSeek(){SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.UserPaint,true);Cursor=Cursors.Hand;}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;int x=8+(int)((Width-16)*(Value/(double)Maximum));using(var bg=new Pen(Color.FromArgb(48,42,105),5)){bg.StartCap=bg.EndCap=System.Drawing.Drawing2D.LineCap.Round;g.DrawLine(bg,8,Height/2,Width-8,Height/2);}if(x>8)using(var gradient=new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(8,0,Math.Max(1,Width-16),Height),Theme.Cyan,Theme.Purple,0f))using(var fg=new Pen(gradient,5)){fg.StartCap=fg.EndCap=System.Drawing.Drawing2D.LineCap.Round;g.DrawLine(fg,8,Height/2,x,Height/2);}using(var b=new SolidBrush(Theme.Text))using(var pen=new Pen(Theme.Cyan,2)){g.FillEllipse(b,x-5,Height/2-5,10,10);g.DrawEllipse(pen,x-5,Height/2-5,10,10);}}
 protected override void OnMouseDown(MouseEventArgs e){Capture=true;Value=(int)(Maximum*Math.Max(0,Math.Min(1,(e.X-8)/(double)Math.Max(1,Width-16))));base.OnMouseDown(e);}
 protected override void OnMouseUp(MouseEventArgs e){Value=(int)(Maximum*Math.Max(0,Math.Min(1,(e.X-8)/(double)Math.Max(1,Width-16))));base.OnMouseUp(e);Capture=false;}
 protected override void OnMouseMove(MouseEventArgs e){if(e.Button==MouseButtons.Left)Value=(int)(Maximum*Math.Max(0,Math.Min(1,(e.X-8)/(double)Math.Max(1,Width-16))));base.OnMouseMove(e);}
}
class Player : Form {
 class OverlayWindow : Form {public Action ToggleFullscreen;protected override bool ProcessCmdKey(ref Message msg,Keys keyData){if(keyData==Keys.Enter&&ToggleFullscreen!=null){ToggleFullscreen();return true;}return base.ProcessCmdKey(ref msg,keyData);}protected override bool ShowWithoutActivation{get{return true;}}}
 OverlayWindow overlay; SlimSeek overlaySeek; Label overlayTime; Button overlayPlay; System.Windows.Forms.Timer overlayTimer;
 bool overlayDragging; Point lastPointer; DateTime lastInteraction=DateTime.UtcNow;
 [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
 void EnsureOverlay(){
  if(overlay!=null)return;
  overlay=new OverlayWindow{FormBorderStyle=FormBorderStyle.None,ShowInTaskbar=false,BackColor=Color.FromArgb(10,16,29),Opacity=0.96,Height=106,StartPosition=FormStartPosition.Manual};
  overlaySeek=new SlimSeek{Dock=DockStyle.Top,Height=36,Maximum=10000,Margin=new Padding(16)};
  overlay.ToggleFullscreen=delegate{Fullscreen();};overlay.ForeColor=Theme.Text;overlay.Padding=new Padding(20,8,20,10);overlay.Controls.Add(overlaySeek);
  var row=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=42,WrapContents=false};overlayPlay=Button(paused?"▶ 播放":"Ⅱ 暂停",delegate{Command("cycle","pause");});((GlowButton)overlayPlay).Accent=true;row.Controls.Add(overlayPlay);
  row.Controls.Add(Button("−10 秒",delegate{Command("seek",-10);}));row.Controls.Add(Button("+10 秒",delegate{Command("seek",10);}));
  overlayTime=new Label{Text=Time(pos)+" / "+Time(duration),Width=165,Height=36,TextAlign=ContentAlignment.MiddleCenter,ForeColor=Color.FromArgb(226,235,248)};row.Controls.Add(overlayTime);
  row.Controls.Add(Button("画面 / 字幕",delegate{Settings();}));row.Controls.Add(Button("退出全屏",delegate{if(full)Fullscreen();}));overlay.Controls.Add(row);
  overlaySeek.MouseDown+=delegate{overlayDragging=true;lastInteraction=DateTime.UtcNow;};
  overlaySeek.MouseUp+=delegate{if(duration>0)Command("seek",overlaySeek.Value/10000.0*duration,"absolute+exact");overlayDragging=false;lastInteraction=DateTime.UtcNow;};
  overlaySeek.MouseCaptureChanged+=delegate{if(!overlaySeek.Capture)overlayDragging=false;};
  overlayTimer=new System.Windows.Forms.Timer{Interval=100};overlayTimer.Tick+=delegate{IntPtr foreground=GetForegroundWindow();UpdateOverlay(Cursor.Position,foreground==Handle||foreground==overlay.Handle||(settingsWindow!=null&&!settingsWindow.IsDisposed&&foreground==settingsWindow.Handle),DateTime.UtcNow);};
 }
 void PositionOverlay(){var screen=Screen.FromControl(this).Bounds;int width=Math.Min(950,Math.Max(760,screen.Width-80));overlay.Bounds=new Rectangle(screen.Left+(screen.Width-width)/2,screen.Bottom-130,width,106);}
 void ShowOverlay(){if(!full)return;EnsureOverlay();PositionOverlay();if(!overlay.Visible)overlay.Show(this);overlay.BringToFront();}
 void UpdateOverlay(Point pointer,bool foreground,DateTime now){
  if(!full){if(overlay!=null)overlay.Hide();return;}
  if(!foreground){overlay.Hide();return;}
  bool moved=pointer!=lastPointer;lastPointer=pointer;
  if(moved){lastInteraction=now;ShowOverlay();}
  if(overlayDragging|| (overlay.Visible&&overlay.Bounds.Contains(pointer)) || (settingsWindow!=null&&!settingsWindow.IsDisposed&&settingsWindow.Visible)){lastInteraction=now;return;}
  if((now-lastInteraction).TotalSeconds>=2.5)overlay.Hide();
 }
 IntPtr pictureHook; MouseHook pictureHookCallback; bool pictureDragArmed,pictureDragging; Point pictureDragStart,pictureWindowStart;
 delegate IntPtr MouseHook(int code,IntPtr message,IntPtr data);
 [DllImport("user32.dll",SetLastError=true)] static extern IntPtr SetWindowsHookEx(int type,MouseHook callback,IntPtr module,uint thread);
 [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(IntPtr hook);
 [DllImport("user32.dll")] static extern IntPtr CallNextHookEx(IntPtr hook,int code,IntPtr message,IntPtr data);
 [DllImport("kernel32.dll",CharSet=CharSet.Auto)] static extern IntPtr GetModuleHandle(string name);
 [DllImport("user32.dll")] static extern IntPtr WindowFromPoint(Point point);
 [DllImport("user32.dll")] static extern IntPtr GetAncestor(IntPtr window,uint flags);
 [DllImport("user32.dll")] static extern bool PhysicalToLogicalPointForPerMonitorDPI(IntPtr window,ref Point point);
 void StartPictureDrag(){pictureHookCallback=PictureMouse;pictureHook=SetWindowsHookEx(14,pictureHookCallback,GetModuleHandle(null),0);if(pictureHook==IntPtr.Zero)status.Text="画面拖动初始化失败："+Marshal.GetLastWin32Error();}
 IntPtr PictureMouse(int code,IntPtr message,IntPtr data){
  if(code>=0){int kind=message.ToInt32();Point pointer=(Point)Marshal.PtrToStructure(data,typeof(Point));PhysicalToLogicalPointForPerMonitorDPI(Handle,ref pointer);
   if(kind==0x202||full||WindowState!=FormWindowState.Normal){pictureDragArmed=false;pictureDragging=false;}
   else if(kind==0x201){pictureDragArmed=video.RectangleToScreen(video.ClientRectangle).Contains(pointer)&&GetAncestor(WindowFromPoint(pointer),2)==Handle;pictureDragging=false;pictureDragStart=pointer;pictureWindowStart=Location;}
   else if(kind==0x200&&pictureDragArmed){int dx=pointer.X-pictureDragStart.X,dy=pointer.Y-pictureDragStart.Y;if(!pictureDragging&&(Math.Abs(dx)>=SystemInformation.DragSize.Width/2||Math.Abs(dy)>=SystemInformation.DragSize.Height/2))pictureDragging=true;if(pictureDragging)Location=new Point(pictureWindowStart.X+dx,pictureWindowStart.Y+dy);}
  }return CallNextHookEx(pictureHook,code,message,data);
 }
 void DragWindow(){if(full)return;ReleaseCapture();SendMessage(Handle,0xA1,(IntPtr)2,IntPtr.Zero);}
 void CacheTracks(){if(file.Length==0||!fileLoaded)return;preferences.Tracks[file]=new SavedTracks{Main=mainSid,Second=secondSid,Position=pos};}
 bool fileLoaded; SavedTracks pendingTracks; bool fitPending; DateTime lastSave=DateTime.UtcNow;
 void VerifyOverlay(){
  if(!full)Fullscreen();var now=DateTime.UtcNow;Point p=new Point(Bounds.Left+40,Bounds.Top+150);lastPointer=new Point(p.X-1,p.Y);
  UpdateOverlay(p,true,now);bool shown=overlay.Visible;
  UpdateOverlay(p,true,now.AddSeconds(3));bool hidden=!overlay.Visible;
  UpdateOverlay(new Point(p.X+2,p.Y),true,now.AddSeconds(4));overlayDragging=true;UpdateOverlay(p,true,now.AddSeconds(8));bool held=overlay.Visible;overlayDragging=false;
  UpdateOverlay(p,true,now.AddSeconds(12));bool rehidden=!overlay.Visible;
  ShowOverlay();int middle=overlaySeek.Width/2;var down=typeof(SlimSeek).GetMethod("OnMouseDown",BindingFlags.Instance|BindingFlags.NonPublic);var up=typeof(SlimSeek).GetMethod("OnMouseUp",BindingFlags.Instance|BindingFlags.NonPublic);down.Invoke(overlaySeek,new object[]{new MouseEventArgs(MouseButtons.Left,1,middle,16,0)});up.Invoke(overlaySeek,new object[]{new MouseEventArgs(MouseButtons.Left,1,middle,16,0)});using(var bitmap=new Bitmap(overlay.Width,overlay.Height)){overlay.DrawToBitmap(bitmap,new Rectangle(0,0,overlay.Width,overlay.Height));bitmap.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"fullscreen-controls.png"));}
  File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"全屏进度条验证.json"),json.Serialize(new{ShownOnMotion=shown,HiddenAfterIdle=hidden,HeldDuringDrag=held,HiddenAfterRelease=rehidden,TargetSeconds=duration*0.5}));
  Fullscreen();
 }

 class SavedTracks{public string Main="auto",Second="no"; public double Position=0;} class Preferences {
  public string LastFile=""; public int WindowX=-1,WindowY=-1,WindowWidth=0,WindowHeight=0; public double Volume=70; public bool Muted=false,Fullscreen=false; public int HdrMode=0; public Dictionary<string,SavedTracks> Tracks=new Dictionary<string,SavedTracks>(); public int Brightness=0, MainPosition=100, SecondPosition=0, MainSize=38, SecondSize=32;
  public string Font="Microsoft YaHei UI", Color="#FFFFFF";
 }
 bool verifying; Preferences preferences=new Preferences(); Form settingsWindow; Panel topBar,bottomBar; string mainSid="auto",secondSid="no";
 string PreferencesPath{get{return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"luma-settings.json");}}
 void ReadPreferences(){try{if(File.Exists(PreferencesPath))preferences=json.Deserialize<Preferences>(File.ReadAllText(PreferencesPath));if(preferences==null)preferences=new Preferences();}catch{preferences=new Preferences();}if(preferences.Tracks==null)preferences.Tracks=new Dictionary<string,SavedTracks>();}
 void SavePreferences(){try{string temp=PreferencesPath+".tmp";File.WriteAllText(temp,json.Serialize(preferences));if(File.Exists(PreferencesPath))File.Replace(temp,PreferencesPath,null);else File.Move(temp,PreferencesPath);}catch{status.Text="设置无法保存，请将播放器放在可写目录。";}}
 void ApplyPreferences(){
  Command("set_property","volume",preferences.Volume);Command("set_property","mute",preferences.Muted);Command("set_property","target-colorspace-hint",preferences.HdrMode==1?"yes":preferences.HdrMode==2?"no":"auto");Command("set_property","target-prim",preferences.HdrMode==2?"bt.709":"auto");Command("set_property","target-trc",preferences.HdrMode==2?"bt.1886":"auto");Command("set_property","brightness",preferences.Brightness);
  Command("set_property","sub-font",preferences.Font);
  Command("set_property","sub-font-size",38);
  Command("set_property","sub-scale",preferences.MainSize/38.0);
  Command("set_property","secondary-sub-scale",preferences.SecondSize/38.0);
  Command("set_property","sub-pos",preferences.MainPosition);
  Command("set_property","secondary-sub-pos",preferences.SecondPosition);
  Command("set_property","sub-color",preferences.Color);
  Command("set_property","sub-ass-override","force");
  Command("set_property","secondary-sub-ass-override","force");
 }
 class TrackChoice {public string Id,Label;public override string ToString(){return Label;}}
 ComboBox TrackPicker(bool secondary){
  var c=new DarkCombo{FlatStyle=FlatStyle.Flat,DropDownStyle=ComboBoxStyle.DropDownList,Width=360,BackColor=Color.FromArgb(20,31,50),ForeColor=Color.FromArgb(226,235,248)};
  c.Items.Add(new TrackChoice{Id="no",Label="关闭"});
  if(!secondary)c.Items.Add(new TrackChoice{Id="auto",Label="自动选择"});
  if(tracks!=null)foreach(object item in tracks){var t=item as Dictionary<string,object>;if(t!=null&&Value(t,"type")=="sub")c.Items.Add(new TrackChoice{Id=Value(t,"id"),Label=Value(t,"id")+" · "+Value(t,"lang")+" · "+Value(t,"title")});}
  string current=secondary?secondSid:mainSid;c.SelectedIndex=0;
  for(int i=0;i<c.Items.Count;i++)if(((TrackChoice)c.Items[i]).Id==current)c.SelectedIndex=i;
  return c;
 }
 void TestSettings(){
  Settings();var rows=(TableLayoutPanel)settingsWindow.Controls[0];
  ((TrackBar)((FlowLayoutPanel)rows.GetControlFromPosition(1,1)).Controls[0]).Value=12;
  var main=(ComboBox)rows.GetControlFromPosition(1,2);var secondary=(ComboBox)rows.GetControlFromPosition(1,3);
  foreach(ComboBox picker in new[]{main,secondary})for(int i=0;i<picker.Items.Count;i++)if(((TrackChoice)picker.Items[i]).Id==(picker==main?"2":"9"))picker.SelectedIndex=i;
  ((NumericUpDown)rows.GetControlFromPosition(1,4)).Value=94;
  ((NumericUpDown)rows.GetControlFromPosition(1,5)).Value=6;
  ((NumericUpDown)rows.GetControlFromPosition(1,6)).Value=44;
  ((NumericUpDown)rows.GetControlFromPosition(1,7)).Value=30;
  ((ComboBox)rows.GetControlFromPosition(1,8)).Text="Segoe UI";
  preferences.Color="#FFE8A3";ApplyPreferences();settingsWindow.Close();
 }
 void Settings(){
  if(settingsWindow!=null&&!settingsWindow.IsDisposed){settingsWindow.Activate();return;}
  var f=new Form{Text="Luma · 画面与字幕",BackColor=Color.FromArgb(10,16,29),ForeColor=Color.FromArgb(226,235,248),Font=new Font("Microsoft YaHei UI",9),ClientSize=new Size(660,690),FormBorderStyle=FormBorderStyle.None,MaximizeBox=false,MinimizeBox=false,StartPosition=FormStartPosition.CenterParent,AutoScroll=false};settingsWindow=f;
  var rows=new TableLayoutPanel{Dock=DockStyle.Top,AutoSize=true,ColumnCount=2,Padding=new Padding(24,18,24,18)};rows.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,156));rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));var settingsTop=new Panel{Dock=DockStyle.Top,Height=40,BackColor=Theme.Base};var closeSettings=Button("×",delegate{f.Close();});closeSettings.Dock=DockStyle.Right;closeSettings.AutoSize=false;closeSettings.Width=40;settingsTop.Controls.Add(closeSettings);settingsTop.Controls.Add(new Label{Text="Luma  /  画面与字幕",Dock=DockStyle.Fill,TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(20,0,0,0),ForeColor=Theme.Cyan});settingsTop.MouseDown+=delegate{ReleaseCapture();SendMessage(f.Handle,0xA1,(IntPtr)2,IntPtr.Zero);};f.Controls.Add(rows);f.Controls.Add(settingsTop);closeSettings.Dock=DockStyle.None;closeSettings.Location=new Point(settingsTop.ClientSize.Width-48,2);settingsTop.Resize+=delegate{closeSettings.Location=new Point(settingsTop.ClientSize.Width-48,2);};closeSettings.Anchor=AnchorStyles.Top|AnchorStyles.Right;closeSettings.BringToFront();
  Action<string,Control> add=delegate(string label,Control c){int r=rows.RowCount++;rows.RowStyles.Add(new RowStyle(SizeType.AutoSize));rows.Controls.Add(new Label{Text=label,AutoSize=true,Margin=new Padding(0,12,8,12)},0,r);c.Margin=new Padding(0,8,0,8);rows.Controls.Add(c,1,r);};
  var heading=new Label{Text="画面与字幕",Font=new Font("Microsoft YaHei UI",11),AutoSize=true};add("",heading);
  var brightPanel=new FlowLayoutPanel{AutoSize=true,WrapContents=false};var bright=new TrackBar{Minimum=-100,Maximum=100,Value=Math.Max(-100,Math.Min(100,preferences.Brightness)),TickStyle=TickStyle.None,Width=260,Height=32};var amount=new Label{Text=bright.Value.ToString(),AutoSize=true,Margin=new Padding(8,8,0,0)};brightPanel.Controls.Add(bright);brightPanel.Controls.Add(amount);bright.ValueChanged+=delegate{preferences.Brightness=bright.Value;amount.Text=bright.Value.ToString();Command("set_property","brightness",bright.Value);};add("亮度 · −100～100",brightPanel);
  var a=TrackPicker(false);var b=TrackPicker(true);add("主字幕",a);add("副字幕",b);
  a.SelectedIndexChanged+=delegate{mainSid=((TrackChoice)a.SelectedItem).Id;if(mainSid!="no"&&mainSid!="auto"&&mainSid==secondSid)b.SelectedIndex=0;Command("set_property","sid",mainSid);};
  b.SelectedIndexChanged+=delegate{secondSid=((TrackChoice)b.SelectedItem).Id;if(secondSid!="no"&&secondSid==mainSid)a.SelectedIndex=0;Command("set_property","secondary-sid",secondSid);};
  Func<int,int,int,Action<int>,NumericUpDown> number=delegate(int value,int min,int max,Action<int> change){var n=new NumericUpDown{BorderStyle=BorderStyle.FixedSingle,Minimum=min,Maximum=max,Value=Math.Max(min,Math.Min(max,value)),Width=120,BackColor=Color.FromArgb(20,31,50),ForeColor=Color.FromArgb(226,235,248)};n.ValueChanged+=delegate{change((int)n.Value);};return n;};
  add("主字幕垂直位置 %",number(preferences.MainPosition,0,100,delegate(int v){preferences.MainPosition=v;Command("set_property","sub-pos",v);}));
  add("副字幕垂直位置 %",number(preferences.SecondPosition,0,100,delegate(int v){preferences.SecondPosition=v;Command("set_property","secondary-sub-pos",v);}));
  add("主字幕字号",number(preferences.MainSize,12,100,delegate(int v){preferences.MainSize=v;Command("set_property","sub-scale",v/38.0);}));
  add("副字幕字号",number(preferences.SecondSize,12,100,delegate(int v){preferences.SecondSize=v;Command("set_property","secondary-sub-scale",v/38.0);}));
  var font=new DarkCombo{FlatStyle=FlatStyle.Flat,Width=260,Text=preferences.Font,BackColor=Color.FromArgb(20,31,50),ForeColor=Color.FromArgb(226,235,248)};font.Items.AddRange(new object[]{"Microsoft YaHei UI","Microsoft YaHei","Segoe UI","SimHei","SimSun","Arial"});font.TextChanged+=delegate{if(font.Text.Trim().Length>0){preferences.Font=font.Text.Trim();Command("set_property","sub-font",preferences.Font);}};add("字幕字体 · 共用",font);
  var color=Button("选择颜色  "+preferences.Color,delegate{});color.Click+=delegate{using(var d=new ColorDialog{FullOpen=true,Color=ColorTranslator.FromHtml(preferences.Color)})if(d.ShowDialog(f)==DialogResult.OK){preferences.Color="#"+d.Color.R.ToString("X2")+d.Color.G.ToString("X2")+d.Color.B.ToString("X2");color.Text="选择颜色  "+preferences.Color;Command("set_property","sub-color",preferences.Color);}};add("字幕颜色 · 共用",color);
  add("",new Label{Text="位置：0 为顶部，100 为底部。建议主字幕 100、副字幕 0。\n字号按 720p 画面缩放；图片字幕无法改变字体及颜色。",AutoSize=true,MaximumSize=new Size(390,0),ForeColor=Color.FromArgb(137,161,191)});
  var buttons=new FlowLayoutPanel{AutoSize=true};buttons.Controls.Add(Button("恢复默认",delegate{preferences.Brightness=0;preferences.MainPosition=100;preferences.SecondPosition=0;preferences.MainSize=38;preferences.SecondSize=32;preferences.Font="Microsoft YaHei UI";preferences.Color="#FFFFFF";ApplyPreferences();SavePreferences();f.Close();Settings();}));buttons.Controls.Add(Button("完成",delegate{f.Close();}));add("",buttons);
  f.FormClosed+=delegate{CacheTracks();SavePreferences();if(settingsWindow==f)settingsWindow=null;};f.Show(this);
 }

 SlimSeek volumeSeek; Panel videoFrame; Panel video=new Panel(); Label title=new Label(), status=new Label(), clock=new Label(); Button play; SlimSeek seek=new SlimSeek(); Button mode; ContextMenuStrip hdrMenu;
 Process process; StreamWriter writer; string pipe="Luma-"+Guid.NewGuid().ToString("N"), file=""; string engine; bool full, paused=true, dragging; double duration,pos; Rectangle previous; JavaScriptSerializer json=new JavaScriptSerializer();
 [DllImport("user32.dll")] static extern bool ReleaseCapture();
 [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h,int m,IntPtr w,IntPtr l);
 [STAThread] static void Main(string[] args){Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new Player(args));}
 Button Button(string text,EventHandler click){var b=new GlowButton{Text=text,FlatStyle=FlatStyle.Flat,ForeColor=Color.FromArgb(226,235,248),BackColor=Color.FromArgb(15,23,40),Font=new Font("Microsoft YaHei UI",9),Cursor=Cursors.Hand,Height=36,AutoSize=true,Padding=new Padding(8,0,8,0)};b.FlatAppearance.BorderSize=0;b.Click+=click;return b;}
 public Player(string[] args){verifying=Array.IndexOf(args,"--verify")>=0;ReadPreferences();Text="Luma";BackColor=Color.FromArgb(10,16,29);ForeColor=Color.FromArgb(226,235,248);Font=new Font("Microsoft YaHei UI",9);Size=new Size(1120,760);MinimumSize=new Size(960,480);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.None;KeyPreview=true;AllowDrop=true;
 var header=topBar=new Panel{Dock=DockStyle.Top,Height=48,Padding=new Padding(18,0,18,6),BackColor=Theme.Base};
 var nav=new Panel{Dock=DockStyle.Top,Height=42,BackColor=Theme.Base};
 var brand=new Label{Text="LUMA",Font=new Font("Segoe UI Semibold",10),Dock=DockStyle.Left,Width=80,TextAlign=ContentAlignment.MiddleLeft,ForeColor=Theme.Cyan};nav.Controls.Add(brand);
 var actions=new FlowLayoutPanel{Dock=DockStyle.Right,Width=430,FlowDirection=FlowDirection.RightToLeft,WrapContents=false,Padding=new Padding(0,4,0,0)};
 foreach(var text in new[]{"×","□","−"}){string action=text;var wb=Button(text,delegate{if(action=="×")Close();else if(action=="−")WindowState=FormWindowState.Minimized;else if(full)Fullscreen();else WindowState=WindowState==FormWindowState.Maximized?FormWindowState.Normal:FormWindowState.Maximized;});wb.AutoSize=false;wb.Width=36;wb.Height=30;actions.Controls.Add(wb);}
 actions.Controls.Add(Button("打开视频",delegate{Open();}));actions.Controls.Add(Button("HDR 设置",delegate{Process.Start("ms-settings:display");}));actions.Controls.Add(Button("全屏",delegate{Fullscreen();}));nav.Controls.Add(actions);
 header.Controls.Add(nav);brand.MouseDown+=delegate{DragWindow();};nav.MouseDown+=delegate{DragWindow();};
 var bottom=bottomBar=new Panel{Dock=DockStyle.Bottom,Height=142,Padding=new Padding(18,2,18,12),BackColor=Theme.Base};
 var progress=new Panel{Dock=DockStyle.Top,Height=38};seek.Dock=DockStyle.Fill;seek.Maximum=10000;seek.BackColor=Theme.Base;seek.MouseDown+=delegate{dragging=true;};seek.MouseUp+=delegate{if(duration>0)Command("seek",seek.Value/10000.0*duration,"absolute+exact");dragging=false;};progress.Controls.Add(seek);
 clock.Text="00:00 / 00:00";clock.TextAlign=ContentAlignment.MiddleRight;clock.Dock=DockStyle.Right;clock.Width=145;clock.ForeColor=Theme.Muted;progress.Controls.Add(clock);
 var transport=new SurfacePanel{Dock=DockStyle.Bottom,Height=62,Padding=new Padding(12,6,12,6)};
 var options=new FlowLayoutPanel{Dock=DockStyle.Left,Width=330,WrapContents=false,Padding=new Padding(0,6,0,0),BackColor=Color.Transparent};
 mode=Button("",delegate{ShowHdrMenu();});mode.AutoSize=false;mode.Width=145;mode.Height=36;mode.AccessibleName="HDR 输出模式";mode.Text=HdrLabel()+"  ⌄";options.Controls.Add(mode);options.Controls.Add(Button("音轨",delegate{Tracks("audio");}));options.Controls.Add(Button("字幕",delegate{Settings();}));transport.Controls.Add(options);
 var audio=new FlowLayoutPanel{Dock=DockStyle.Right,Width=200,WrapContents=false,Padding=new Padding(0,6,0,0),BackColor=Color.Transparent};audio.Controls.Add(Button("静音",delegate{Command("cycle","mute");}));volumeSeek=new SlimSeek{Width=115,Height=30,Maximum=130,Value=(int)preferences.Volume,BackColor=Theme.Surface};volumeSeek.MouseUp+=delegate{Command("set_property","volume",volumeSeek.Value);};audio.Controls.Add(volumeSeek);transport.Controls.Add(audio);
 var center=new FlowLayoutPanel{Width=186,Height=50,WrapContents=false,BackColor=Color.Transparent};
 var back=Button("−10",delegate{Command("seek",-10);});back.AutoSize=false;back.Width=52;back.Height=40;back.Margin=new Padding(2,5,2,0);center.Controls.Add(back);
 play=Button("▶",delegate{Command("cycle","pause");});play.AutoSize=false;play.Width=48;play.Height=48;play.Font=new Font("Segoe UI",15);((GlowButton)play).Accent=true;((GlowButton)play).RoundPlay=true;play.AccessibleName="播放或暂停";center.Controls.Add(play);
 var forward=Button("+10",delegate{Command("seek",10);});forward.AutoSize=false;forward.Width=52;forward.Height=40;forward.Margin=new Padding(2,5,2,0);center.Controls.Add(forward);transport.Controls.Add(center);center.BringToFront();transport.Resize+=delegate{center.Location=new Point((transport.ClientSize.Width-center.Width)/2,6);};
 status.Text="就绪";status.ForeColor=Theme.Muted;status.Dock=DockStyle.Fill;status.Font=new Font("Microsoft YaHei UI",8);status.TextAlign=ContentAlignment.MiddleLeft;
 bottom.Controls.Add(status);bottom.Controls.Add(progress);bottom.Controls.Add(transport);
 video.Dock=DockStyle.Fill;video.BackColor=Color.Black;video.DoubleClick+=delegate{Fullscreen();};video.Paint+=delegate(object s,PaintEventArgs e){if(process==null){using(var f=new Font("Microsoft YaHei UI",13))using(var brush=new SolidBrush(Color.FromArgb(160,160,160)))e.Graphics.DrawString("拖入视频，或按 Ctrl+O 打开",f,brush,new PointF(65,100));}};
 videoFrame=new Panel{Dock=DockStyle.Fill,Padding=new Padding(18,0,18,0),BackColor=Theme.Base};videoFrame.Controls.Add(video);Controls.Add(videoFrame);Controls.Add(bottom);Controls.Add(header);DragEnter+=delegate(object s,DragEventArgs e){if(e.Data.GetDataPresent(DataFormats.FileDrop))e.Effect=DragDropEffects.Copy;};DragDrop+=delegate(object s,DragEventArgs e){LoadVideo(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);};
 KeyDown+=delegate(object s,KeyEventArgs e){if(e.KeyCode==Keys.Space){Command("cycle","pause");e.Handled=true;}if(e.KeyCode==Keys.F)Fullscreen();if(e.KeyCode==Keys.Escape&&full)Fullscreen();if(e.KeyCode==Keys.Up)Command("add","volume",5);if(e.KeyCode==Keys.Down)Command("add","volume",-5);if(e.KeyCode==Keys.Left)Command("seek",-10);if(e.KeyCode==Keys.Right)Command("seek",10);if(e.KeyCode==Keys.O&&e.Control)Open();};
 FormClosing+=delegate{if(pictureHook!=IntPtr.Zero){UnhookWindowsHookEx(pictureHook);pictureHook=IntPtr.Zero;}Rectangle r=full?previous:Bounds;if(WindowState==FormWindowState.Normal){preferences.WindowX=r.X;preferences.WindowY=r.Y;preferences.WindowWidth=r.Width;preferences.WindowHeight=r.Height;}CacheTracks();SavePreferences();if(overlayTimer!=null)overlayTimer.Stop();if(overlay!=null)overlay.Close();Command("quit");try{if(process!=null&&!process.HasExited){if(!process.WaitForExit(1200))process.Kill();}}catch{};};
 Shown+=delegate{StartPictureDrag();if(args.Length>0 && (args[0]=="--preview" || args[0]=="--preview-settings")){if(args[0]=="--preview-settings")Settings();var timer=new System.Windows.Forms.Timer{Interval=600};timer.Tick+=delegate{timer.Stop();Form target=settingsWindow??this;using(var bitmap=new Bitmap(target.Width,target.Height)){target.DrawToBitmap(bitmap,new Rectangle(0,0,target.Width,target.Height));bitmap.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,args[0]=="--preview-settings"?"settings-preview.png":"preview.png"));}Close();};timer.Start();return;}try{Extract();StartEngine();if(args.Length>0 && args[0]!="--verify")LoadVideo(args[0]);else if(File.Exists(preferences.LastFile))LoadVideo(preferences.LastFile);if(preferences.Fullscreen)Fullscreen();}catch(Exception ex){status.Text="启动失败";MessageBox.Show(ex.Message,"Luma",MessageBoxButtons.OK,MessageBoxIcon.Error);}};
 }
 void Extract(){string dir=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,".luma-runtime","engine-20261015");Directory.CreateDirectory(dir);engine=Path.Combine(dir,"mpv.exe");if(!File.Exists(engine)){using(Stream input=Assembly.GetExecutingAssembly().GetManifestResourceStream("mpv.exe")){if(input==null)throw new Exception("缺少 mpv 播放内核。");string temp=engine+"."+Guid.NewGuid().ToString("N")+".tmp";using(var output=File.Create(temp))input.CopyTo(output);try{File.Move(temp,engine);}catch{File.Delete(temp);if(!File.Exists(engine))throw;}}}}
 string Quote(string value){return "\""+value.Replace("\"","\\\"")+"\"";}
 void StartEngine(){string bindings=Path.Combine(Path.GetDirectoryName(engine),"input-"+pipe+".conf");File.WriteAllText(bindings,"f script-message luma-fullscreen\nENTER script-message luma-fullscreen\nESC script-message luma-exit-fullscreen\nMBTN_LEFT_DBL script-message luma-fullscreen\n");string dragScript=Path.Combine(Path.GetDirectoryName(engine),"window-drag.lua");File.WriteAllText(dragScript,"-- Window dragging is handled by the host.\n");string log=Path.Combine(Path.GetDirectoryName(engine),"playback-"+pipe+".log");process=new Process();process.StartInfo=new ProcessStartInfo(engine,"--no-config --pause=yes --idle=yes --keep-open=yes --osc=no --input-default-bindings=yes --input-vo-keyboard=yes --vo=gpu-next --gpu-api=d3d11 --gpu-context=d3d11 --hwdec=auto-safe --target-colorspace-hint=auto --tone-mapping=bt.2390 --hdr-compute-peak=yes --dither-depth=auto --volume=70 --msg-level=all=warn,cplayer=info,vd=info,vo/gpu-next=info --sub-auto=fuzzy --slang=zh-Hans,zh,chi,eng --alang=spa,es,eng --script="+Quote(dragScript)+" --input-conf="+Quote(bindings)+" --wid="+video.Handle.ToInt64()+" --input-ipc-server="+pipe+" --log-file="+Quote(log)){UseShellExecute=false,CreateNoWindow=true};process.Start();
 var thread=new Thread(delegate(){try{var client=new NamedPipeClientStream(".",pipe,PipeDirection.InOut,PipeOptions.Asynchronous);client.Connect(20000);writer=new StreamWriter(client){AutoFlush=true};Command("enable_event","client-message");Command("observe_property",1,"time-pos");Command("observe_property",2,"duration");Command("observe_property",3,"pause");Command("observe_property",4,"video-params");Command("observe_property",5,"track-list");Command("observe_property",6,"video-target-params");Command("observe_property",7,"sid");Command("observe_property",8,"secondary-sid");Command("observe_property",9,"volume");Command("observe_property",10,"mute");Command("observe_property",11,"path");ApplyPreferences();if(file!="")Command("loadfile",file,"replace");using(var reader=new StreamReader(client)){string line;while((line=reader.ReadLine())!=null){var data=json.Deserialize<Dictionary<string,object>>(line);BeginInvoke((Action)delegate{Event(data);});}}}catch(Exception ex){if(!IsDisposed)try{BeginInvoke((Action)delegate{status.Text="播放连接中断："+ex.Message;});}catch{}}});thread.IsBackground=true;thread.Start();}
 void Command(params object[] args){try{if(writer!=null)lock(writer)writer.WriteLine(json.Serialize(new Dictionary<string,object>{{"command",args}}));}catch{}}
 void Open(){using(var d=new OpenFileDialog{Filter="视频文件|*.mkv;*.mp4;*.mov;*.avi;*.webm;*.m2ts;*.ts|所有文件|*.*"})if(d.ShowDialog()==DialogResult.OK)LoadVideo(d.FileName);}
 void LoadVideo(string path){if(!File.Exists(path))return;CacheTracks();fileLoaded=false;file=Path.GetFullPath(path);preferences.LastFile=file;preferences.Tracks.TryGetValue(file,out pendingTracks);fitPending=true;pos=0;duration=0;title.Text=Path.GetFileNameWithoutExtension(file);Text="Luma · "+title.Text;Command("loadfile",file,"replace");Command("set_property","pause",true);}
 Dictionary<string,object> source,output; object[] tracks;
 string Value(Dictionary<string,object> d,string k){return d!=null&&d.ContainsKey(k)?Convert.ToString(d[k]):"—";}
 void Event(Dictionary<string,object> e){if(Value(e,"event")=="file-loaded"){fileLoaded=true;if(pendingTracks!=null){Command("set_property","sid",pendingTracks.Main);Command("set_property","secondary-sid",pendingTracks.Second);if(pendingTracks.Position>0)Command("seek",pendingTracks.Position,"absolute+exact");}Command("set_property","pause",true);}if(Value(e,"event")=="client-message" && e.ContainsKey("args")){var args=e["args"] as object[];var argList=e["args"] as System.Collections.ArrayList;if(argList!=null)args=argList.ToArray();if(args!=null&&args.Length>0){if(verifying&&Convert.ToString(args[0])=="luma-close"){Close();return;}if(verifying&&Convert.ToString(args[0])=="luma-test-overlay")VerifyOverlay();if(verifying&&Convert.ToString(args[0])=="luma-diagnostics")File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"全屏验证.json"),json.Serialize(new {Fullscreen=full,TopVisible=topBar.Visible,BottomVisible=bottomBar.Visible,Bounds=Bounds.ToString(),Screen=Screen.FromControl(this).Bounds.ToString()}));if(verifying&&Convert.ToString(args[0])=="luma-test-settings")TestSettings();if(Convert.ToString(args[0])=="luma-fullscreen")Fullscreen();if(Convert.ToString(args[0])=="luma-exit-fullscreen"&&full)Fullscreen();}return;}if(Value(e,"event")=="end-file"&&Value(e,"reason")=="error"){status.Text="播放失败，请查看播放日志";return;}if(!e.ContainsKey("name")||!e.ContainsKey("data")||e["data"]==null)return;string name=Value(e,"name");object d=e["data"];if(name=="path"&&Convert.ToString(d)!=file&&File.Exists(Convert.ToString(d))){CacheTracks();file=Path.GetFullPath(Convert.ToString(d));preferences.LastFile=file;preferences.Tracks.TryGetValue(file,out pendingTracks);fitPending=true;pos=0;duration=0;title.Text=Path.GetFileNameWithoutExtension(file);Text="Luma · "+title.Text;}if(name=="volume"){preferences.Volume=Convert.ToDouble(d);if(volumeSeek!=null&&!volumeSeek.Capture)volumeSeek.Value=(int)preferences.Volume;SavePreferences();}if(name=="mute"){preferences.Muted=Convert.ToBoolean(d);SavePreferences();}if(name=="sid")mainSid=Convert.ToString(d);if(name=="secondary-sid")secondSid=Convert.ToString(d);if(name=="time-pos"){pos=Convert.ToDouble(d);if(fileLoaded&&(DateTime.UtcNow-lastSave).TotalSeconds>=5){CacheTracks();SavePreferences();lastSave=DateTime.UtcNow;}}if(name=="duration")duration=Convert.ToDouble(d);if(name=="pause"){paused=Convert.ToBoolean(d);if(fileLoaded){CacheTracks();SavePreferences();}play.Text=paused?"▶":"Ⅱ";}if(name=="video-params"){source=d as Dictionary<string,object>;if(fitPending&&source!=null){if(FitVideo())fitPending=false;}}if(name=="video-target-params")output=d as Dictionary<string,object>;if(name=="track-list"){var a=d as System.Collections.ArrayList;if(a!=null)tracks=a.ToArray();else tracks=d as object[];}clock.Text=Time(pos)+" / "+Time(duration);if(overlay!=null){overlayTime.Text=clock.Text;overlayPlay.Text=paused?"▶ 播放":"Ⅱ 暂停";if(!overlayDragging&&duration>0)overlaySeek.Value=Math.Max(0,Math.Min(10000,(int)(pos/duration*10000)));}if(!dragging&&duration>0)seek.Value=Math.Max(0,Math.Min(10000,(int)(pos/duration*10000)));if(source!=null)status.Text=Value(source,"w")+" × "+Value(source,"h")+"  ·  "+Value(source,"primaries")+" / "+Value(source,"gamma")+"   →   输出 "+Value(output,"primaries")+" / "+Value(output,"gamma");}
 string Time(double seconds){var t=TimeSpan.FromSeconds(Math.Max(0,seconds));return t.TotalHours>=1?t.ToString(@"h\:mm\:ss"):t.ToString(@"mm\:ss");}
 void Tracks(string type){var menu=new ContextMenuStrip{BackColor=Theme.Surface,ForeColor=Theme.Text,ShowImageMargin=false};if(type=="sub")menu.Items.Add("关闭字幕",null,delegate{Command("set_property","sid","no");});if(tracks!=null)foreach(object item in tracks){var t=item as Dictionary<string,object>;if(t==null||Value(t,"type")!=type)continue;object id=t["id"];menu.Items.Add(Value(t,"id")+" · "+Value(t,"lang")+" · "+Value(t,"title"),null,delegate{Command("set_property",type=="sub"?"sid":"aid",id);});}menu.Show(Cursor.Position);}
 string HdrLabel(){return preferences.HdrMode==1?"HDR · 强制 HDR10":preferences.HdrMode==2?"SDR · 色调映射":"HDR · 自动适配";}
 void ShowHdrMenu(){if(hdrMenu==null){hdrMenu=new ContextMenuStrip{BackColor=Theme.Surface,ForeColor=Theme.Text,ShowImageMargin=false,Renderer=new ToolStripProfessionalRenderer(new DarkMenuColors())};foreach(var label in new[]{"HDR · 自动适配","HDR · 强制 HDR10","SDR · 色调映射"}){int index=hdrMenu.Items.Count;hdrMenu.Items.Add(label,null,delegate{preferences.HdrMode=index;ApplyMode();});}}for(int i=0;i<hdrMenu.Items.Count;i++)((ToolStripMenuItem)hdrMenu.Items[i]).Checked=i==preferences.HdrMode;hdrMenu.Show(mode,new Point(0,mode.Height+4));}
 void ApplyMode(){mode.Text=HdrLabel()+"  ⌄";SavePreferences();Command("set_property","target-colorspace-hint",preferences.HdrMode==1?"yes":preferences.HdrMode==2?"no":"auto");Command("set_property","target-prim",preferences.HdrMode==2?"bt.709":"auto");Command("set_property","target-trc",preferences.HdrMode==2?"bt.1886":"auto");}
 protected override bool ProcessCmdKey(ref Message msg,Keys keyData){if(keyData==Keys.Enter){Fullscreen();return true;}return base.ProcessCmdKey(ref msg,keyData);}
 protected override void WndProc(ref Message m){base.WndProc(ref m);if(m.Msg==0x84&&!full){Point p=PointToClient(new Point(m.LParam.ToInt32()));if(p.X>=ClientSize.Width-12&&p.Y>=ClientSize.Height-12)m.Result=(IntPtr)17;}}

 bool FitVideo(){
  double w,h;if(source==null||!double.TryParse(Value(source,"dw"),out w)||!double.TryParse(Value(source,"dh"),out h)||w<=0||h<=0)return false;
  Rectangle area=Screen.FromControl(this).WorkingArea;int chrome=topBar.Height+bottomBar.Height;int gutter=full?36:videoFrame.Padding.Horizontal;
  double ratio=w/h;int width=Math.Min(Math.Max(1120,MinimumSize.Width),area.Width-40);int height=(int)Math.Round((width-gutter)/ratio)+chrome;
  if(height>area.Height-40){height=area.Height-40;width=(int)Math.Round((height-chrome)*ratio)+gutter;}
  width=Math.Max(MinimumSize.Width,width);height=Math.Max(MinimumSize.Height,height);
  Rectangle target=new Rectangle(area.Left+(area.Width-width)/2,area.Top+(area.Height-height)/2,width,height);
  if(full)previous=target;else {WindowState=FormWindowState.Normal;Bounds=target;}return true;
 }
 void Fullscreen(){if(!full){previous=Bounds;FormBorderStyle=FormBorderStyle.None;WindowState=FormWindowState.Normal;Bounds=Screen.FromControl(this).Bounds;full=true;preferences.Fullscreen=true;topBar.Hide();bottomBar.Hide();videoFrame.Padding=Padding.Empty;EnsureOverlay();lastPointer=Cursor.Position;lastInteraction=DateTime.UtcNow;ShowOverlay();overlayTimer.Start();}else{FormBorderStyle=FormBorderStyle.None;Bounds=previous;full=false;preferences.Fullscreen=false;overlayTimer.Stop();overlay.Hide();topBar.Show();bottomBar.Show();videoFrame.Padding=new Padding(18,0,18,0);}}
}











