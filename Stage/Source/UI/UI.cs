using Stage.Core;
using Stage.ImGui;
using Stage.Rendering;

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stage.UIModule
{
    public static class UI
    {
        public static unsafe bool Begin(string title, WindowFlags flags = 0)
        {
            return _imgui.Begin(title, null, (int)flags);
        }

        public static unsafe bool Begin(string title, ref bool showClose, WindowFlags flags = 0)
        {
            bool* ptr = (bool*)Unsafe.AsPointer(ref showClose);
            return _imgui.Begin(title, ptr, (int)flags);
        }

        public static unsafe void End()
        {
            _imgui.End();
        }

        public static unsafe void Text(string text)
        {
            _imgui.Text(text);
        }

        public static unsafe void Text(string format, params object?[] objects)
        {
            format = string.Format(format, objects);

            _imgui.Text(format);
        }

        public static unsafe void PushID(string id)
        {
            _imgui.PushID_Str(id);
        }

        public static unsafe void PopID()
        {
            _imgui.PopID();
        }

        public static unsafe void PushStyleVar(StyleVar styleVar, float value)
        {
            _imgui.PushStyleVar_Float((int)styleVar, value);
        }

        public static unsafe void PushStyleVar(StyleVar styleVar, Vector2 value)
        {
            _imgui.PushStyleVar_Vec2((int)styleVar, &value);
        }

        public static unsafe void PopStyleVar(int count = 1)
        {
            _imgui.PopStyleVar(count);
        }

        public static unsafe void PushStyleColour(StyleColour col, Vector4 value)
        {
            _imgui.PushStyleColor_Vec4((int)col, &value);
        }

        public static unsafe void PopStyleColour(int count = 1)
        {
            _imgui.PopStyleColor(count);
        }

        public static unsafe void CreateDockspace(Menubar? menubar)
        {
            WindowFlags flags = WindowFlags.MenuBar | WindowFlags.NoDocking;
            flags |= WindowFlags.NoTitleBar | WindowFlags.NoResize | WindowFlags.NoMove;
            flags |= WindowFlags.NoBringToFrontOnFocus | WindowFlags.NoNavFocus;

            nint viewport = _imgui.GetMainViewport();
            Vector2 pivot = Vector2.Zero;
            _imgui.SetNextWindowPos(_helper.ImGuiGetImViewportPos(viewport), 0, &pivot);
            _imgui.SetNextWindowSize(_helper.ImGuiGetImViewportSize(viewport), 0);
            _imgui.SetNextWindowViewport(_helper.ImGuiGetImViewportID(viewport));
            PushStyleVar(StyleVar.WindowRounding, 0.0f);
            PushStyleVar(StyleVar.WindowBorderSize, 0.0f);

            PushStyleVar(StyleVar.WindowPadding, new Vector2(0));
            Begin("Dockspace", flags);
            PopStyleVar(3);

            float minWinSizeX = _helper.ImGuiGetMinWinSizeX();
            _helper.ImGuiSetMinWinSizeX(370.0f);
            uint id = _imgui.GetID("StageDockSpace");
            Vector2 size = Vector2.Zero;
            _imgui.DockSpace(id, &size, 0, nint.Zero);
            _helper.ImGuiSetMinWinSizeX(minWinSizeX);

            if (menubar != null)
                menubar.Render();

            End();
        }

        public static unsafe bool ImageButton(Texture texture, string id, Vector2? size = null, Vector2? uv0 = null, Vector2? uv1 = null, Vector4? bgCol = null, Vector4? tintCol = null)
        {
            Vector2 newSize = size ?? new Vector2(texture.Width, texture.Height);
            Vector2 newUv0 = uv0 ?? new Vector2(0);
            Vector2 newUv1 = uv1 ?? new Vector2(1);
            Vector4 newBgCol = bgCol ?? new Vector4(0);
            Vector4 newTintCol = tintCol ?? new Vector4(1);

            return _imgui.ImageButton(id, (nint)texture.GetID(), &newSize, &newUv0, &newUv1, &newBgCol, &newTintCol);
        }

        public static unsafe void Columns(int count, string id, bool border = true)
        {
            _imgui.Columns(count, id, border);
        }

        public static unsafe void Columns(int count, bool border = true)
        {
            _imgui.Columns(count, null, border);
        }

        public static unsafe void SetColumnWidth(int index, float width)
        {
            _imgui.SetColumnWidth(index, width);
        }

        public static unsafe void NextColumn()
        {
            _imgui.NextColumn();
        }

        public static unsafe Vector2 GetCurrentCursorPos()
        {
            _imgui.GetCursorScreenPos(out Vector2 val);
            return val;
        }

        public static unsafe Vector2 GetCursorPos()
        {
            _imgui.GetCursorPos(out Vector2 val);
            return val;
        }

        internal static unsafe DrawList GetWindowDrawList()
        {
            nint ptr = _imgui.GetWindowDrawList();
            return new DrawList(ptr);
        }

        public static unsafe bool InputText(string label, ref string input, InputTextFlags flags = 0)
        {
            byte[] buffer = new byte[input.Length + 10];
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                buffer[i] = inputBytes[i];
            }

            bool result = false;

            fixed (byte* buf = buffer)
            {
                result = _imgui.InputText(label, buf, (ulong)buffer.Length, (int)flags, null, null);
            }

            input = Encoding.UTF8.GetString(buffer);

            return result;
        }

        public static unsafe bool InputText(string label, ref string input, InputTextCallback callback, void* userData)
        {
            byte[] buffer = new byte[input.Length + 10];
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                buffer[i] = inputBytes[i];
            }

            bool result = false;

            fixed (byte* buf = buffer)
            {
                result = _imgui.InputText(label, buf, (ulong)buffer.Length, 0, callback, userData);
            }

            input = Encoding.UTF8.GetString(buffer);

            return result;
        }

        public static unsafe bool InputText(string label, ref string input, string placeholder, InputTextFlags flags = 0)
        {
            byte[] buffer = new byte[input.Length + 10];
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                buffer[i] = inputBytes[i];
            }

            bool result = false;

            fixed (byte* buf = buffer)
            {
                result = _imgui.InputTextWithHint(label, placeholder, buf, (ulong)buffer.Length, (int)flags, null, null);
            }

            input = Encoding.UTF8.GetString(buffer);

            return result;
        }

        public static unsafe bool InputText(string label, ref string input, string placeholder, InputTextCallback callback, void* userData)
        {
            byte[] buffer = new byte[input.Length + 10];
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                buffer[i] = inputBytes[i];
            }

            bool result = false;

            fixed (byte* buf = buffer)
            {
                result = _imgui.InputTextWithHint(label, placeholder, buf, (ulong)buffer.Length, 0, callback, userData);
            }

            input = Encoding.UTF8.GetString(buffer);

            return result;
        }

        public static unsafe bool InputText(string label, ref string input,
            InputTextFlags flags, InputTextCallback callback, void* userData)
        {
            byte[] buffer = new byte[input.Length + 10];
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            for (int i = 0; i < inputBytes.Length; i++)
            {
                buffer[i] = inputBytes[i];
            }

            bool result = false;

            fixed (byte* buf = buffer)
            {
                result = _imgui.InputText(label, buf, (ulong)buffer.Length, (int)flags, callback, userData);
            }

            input = Encoding.UTF8.GetString(buffer);

            return result;
        }

        public static unsafe void PushItemWidth(float width)
        {
            _imgui.PushItemWidth(width);
        }

        public static unsafe void PopItemWidth()
        {
            _imgui.PopItemWidth();
        }

        public static unsafe void SameLine(float offsetFromStartX = 0.0f, float spacing = -1.0f)
        {
            _imgui.SameLine(offsetFromStartX, spacing);
        }

        public static unsafe bool Button(string label)
        {
            Vector2 size = Vector2.Zero;
            return _imgui.Button(label, &size);
        }

        public static unsafe bool Button(string label, Vector2 size)
        {
            return _imgui.Button(label, &size);
        }

        public static unsafe bool Button(string label, ButtonFlags flags)
        {
            Vector2 size = Vector2.Zero;
            return _imgui.ButtonEx(label, &size, (int)flags);
        }
        
        public static unsafe bool InvisibleButton(string id, Vector2 size, ButtonFlags flags = 0)
        {
            return _imgui.InvisibleButton(id, &size, (int)flags);
        }

        public static unsafe bool IsItemHovered()
        {
            // TODO: hovered flags
            return _imgui.IsItemHovered(0);
        }

        public static unsafe bool IsItemActive()
        {
            return _imgui.IsItemActive();
        }

        public static unsafe Vector2 GetMousePos()
        {
            // TODO: Move to ImGuiIO
            _helper.ImGuiIO_GetMousePos(out Vector2 value);
            return value;
        }

        public static unsafe bool IsMouseClicked(MouseButton button, bool repeat = false)
        {
            return _imgui.IsMouseClicked((int)button, repeat);
        }

        public static unsafe bool IsMouseDown(MouseButton button)
        {
            return _imgui.IsMouseDown((int)button);
        }

        public static unsafe bool IsMouseDragging(MouseButton button, float lockThreshold = -1.0f)
        {
            return _imgui.IsMouseDragging((int)button, lockThreshold);
        }

        public static unsafe Vector2 GetMouseDragDelta(MouseButton button, float lockThreshold = -1.0f)
        {
            _imgui.GetMouseDragDelta(out Vector2 value, (int)button, lockThreshold);
            return value;
        }

        public static unsafe Vector2 GetMouseDelta()
        {
            // TODO: Move to ImGuiIO
            _helper.ImGuiIO_GetMouseDelta(out Vector2 value);
            return value;
        }

        public static unsafe bool MenuItem(string label, string shortcut = "", bool selected = false, bool enabled = true)
        {
            return _imgui.MenuItem_Bool(label, shortcut, selected, enabled);
        }

        public static unsafe void OpenPopupOnItemClick(string id, MouseButton flags = 0)
        {
            _imgui.OpenPopupOnItemClick(id, (int)flags);
        }

        public static unsafe bool BeginPopup(string id, WindowFlags flags = 0)
        {
            return _imgui.BeginPopup(id, (int)flags);
        }

        public static unsafe bool Button(string label, Vector2 size, ButtonFlags flags)
        {
            return _imgui.ButtonEx(label, &size, (int)flags);
        }

        public static unsafe bool Checkpoint(string label, ref bool value)
        {
            bool* v = (bool*)Unsafe.AsPointer(ref value);
            return _imgui.Checkbox(label, v);
        }

        public static unsafe bool BeginTable(string id, int numColumns, TableFlags flags = 0, float innerWidth = 0.0f)
        {
            Vector2 vector = Vector2.Zero;
            return _imgui.BeginTable(id, numColumns, (int)flags, &vector, innerWidth);
        }

        public static unsafe bool BeginTable(string id, int numColumns, Vector2 outerSize, float innerWidth = 0.0f)
        {
            return _imgui.BeginTable(id, numColumns, 0, &outerSize, innerWidth);
        }

        public static unsafe bool BeginTable(string id, int numColumns, TableFlags flags, Vector2 outerSize, float innerWidth = 0.0f)
        {
            return _imgui.BeginTable(id, numColumns, (int)flags, &outerSize, innerWidth);
        }

        public static unsafe void EndTable()
        {
            _imgui.EndTable();
        }

        public static unsafe void TableSetupColumn(string label, TableColumnFlags flags = 0, float initWidthOrHeight = 0.0f, uint userID = 0)
        {
            _imgui.TableSetupColumn(label, (int)flags, initWidthOrHeight, userID);
        }

        public static unsafe Vector2 CalcTextSize(string text, bool hideAfterDoubleHash = false, float wrapWidth = -1.0f)
        {
            _imgui.CalcTextSize(out Vector2 size, text, nint.Zero, hideAfterDoubleHash, wrapWidth);
            return size;
        }

        public static unsafe Vector2 GetWindowSize()
        {
            _imgui.GetWindowSize(out Vector2 size);
            return size;
        }

        public static unsafe void TableNextRow(TableRowFlags flags = 0, float minRowHeight = 0.0f)
        {
            _imgui.TableNextRow((int)flags, minRowHeight);
        }

        public static unsafe void TableSetBgColour(TableBgTarget target, Vector4 colour, int columnNumber = -1)
        {
            _imgui.TableSetBgColor((int)target, GetColourU32(colour), columnNumber);
        }

        public static unsafe void TableSetBgColour(TableBgTarget target, uint colour, int columnNumber = -1)
        {
            _imgui.TableSetBgColor((int)target, colour, columnNumber);
        }

        public static unsafe bool TableSetColumnIndex(int index)
        {
            return _imgui.TableSetColumnIndex(index);
        }

        public static unsafe uint GetColourU32(Vector4 colour)
        {
            colour.W *= _helper.ImGuiStyle_GetAlpha();

            uint value;
            value =  (uint)((int)(((colour.X < 0.0f) ? 0.0f : (colour.X > 1.0f) ? 1.0f : colour.X) * 255.0f + 0.5f)) << 16;
            value |= (uint)((int)(((colour.Y < 0.0f) ? 0.0f : (colour.Y > 1.0f) ? 1.0f : colour.Y) * 255.0f + 0.5f)) << 8;
            value |= (uint)((int)(((colour.Z < 0.0f) ? 0.0f : (colour.Z > 1.0f) ? 1.0f : colour.Z) * 255.0f + 0.5f)) << 0;
            value |= (uint)((int)(((colour.W < 0.0f) ? 0.0f : (colour.W > 1.0f) ? 1.0f : colour.W) * 255.0f + 0.5f)) << 24;

            return value;
        }

        public static unsafe void AlignText(float offset = 1.0f)
        {
            _helper.ImGuiAlignText(offset);
        }

        public static unsafe void ScrollToBottom()
        {
            _helper.ImGuiScrollToBottom();
        }

        public static unsafe float GetScrollY()
        {
            return _imgui.GetScrollY();
        }

        public static unsafe float GetScrollMaxY()
        {
            return _imgui.GetScrollMaxY();
        }

        public static unsafe void OpenModal(string id, PopupFlags flags = 0)
        {
            _imgui.OpenPopup(id, (int)flags);
        }

        public static unsafe bool BeginModalPopup(string name, ref bool open, WindowFlags flags = 0)
        {
            bool* p_open = (bool*)Unsafe.AsPointer(ref open);
            return _imgui.BeginPopupModal(name, p_open, (int)flags);
        }

        public static unsafe void SetNextWindowSize(Vector2 size)
        {
            _imgui.SetNextWindowSize(&size, 0);
        }

        public static unsafe void SetNextWindowPos(Vector2 pos, UICondition condition = 0)
        {
            Vector2 pivot = Vector2.Zero;
            _imgui.SetNextWindowPos(&pos, (int)condition, &pivot);
        }

        public static unsafe void SetNextWindowPos(Vector2 pos, Vector2 pivot)
        {
            _imgui.SetNextWindowPos(&pos, 0, &pivot);
        }

        public static unsafe void SetNextWindowPos(Vector2 pos, UICondition condition, Vector2 pivot)
        {
            _imgui.SetNextWindowPos(&pos, (int)condition, &pivot);
        }

        public static unsafe bool BeginModalPopup(string name, WindowFlags flags = 0)
        {
            return _imgui.BeginPopupModal(name, null, (int)flags);
        }

        public static unsafe void CloseCurrentPopup()
        {
            _imgui.CloseCurrentPopup();
        }

        public static unsafe void EndPopup()
        {
            _imgui.EndPopup();
        }

        public static unsafe Vector2 GetCurrentViewportCenter()
        {
            _imgui.ImGuiViewport_GetCenter(out Vector2 center, _imgui.GetWindowViewport());
            return center;
        }

        public static unsafe void SetCursorPosX(float local_x)
        {
            _imgui.SetCursorPosX(local_x);
        }

        public static unsafe void SetCursorPosY(float local_y)
        {
            _imgui.SetCursorPosY(local_y);
        }

        public static unsafe void SetCursorPos(Vector2 local_pos)
        {
            _imgui.SetCursorPosX(local_pos.X);
            _imgui.SetCursorPosY(local_pos.Y);
        }

        public static unsafe Vector2 GetContentRegionAvailable()
        {
            _imgui.GetContentRegionAvail(out Vector2 size);
            return size;
        }

        private static short m_ModalCount = 0;
        private static Window? m_ModalWindow = null;
        private static bool m_CloseModal = false;

        /// <summary>
        /// <para>Begin a modal dialog. Cannot be called in OnUI().</para>
        /// <para>Only one modal dialog can be open at any time, opening more will throw a <see cref="System.InvalidOperationException"/>.</para>
        /// </summary>
        public static unsafe bool BeginModal(string title, Vector2 size, bool freezeOtherWindows = true)
        {
            if (ImGuiImpl.HasBegun)
            {
                throw new InvalidOperationException("BeginModal cannot be called in OnUI.");
            }
            if (m_ModalCount > 1)
            {
                throw new InvalidOperationException("Cannot open more than one modal!");
            }

            if (m_ModalWindow == null)
            {
                m_ModalWindow = new Window(
                    new WindowProperties { Width = (uint)size.X, Height = (uint)size.Y, Title = title, Maximised = false },
                    secondaryWindow: true, freezeOtherWindows: true);
            }

            if (!m_ModalWindow.ShouldClose)
            {
                Window.SecondWindowBeginUI(Application.Instance.Window, m_ModalWindow);

                WindowFlags flags = WindowFlags.NoDocking | WindowFlags.NoDecoration | WindowFlags.NoMove;
                flags |= WindowFlags.NoBringToFrontOnFocus | WindowFlags.NoNavFocus;

                SetNextWindowSize(DisplaySize());
                SetNextWindowPos(GetCurrentViewportCenter(), new Vector2(0.5f));

                m_ModalCount++;
                Begin(title, flags);
                return true;
            }
            else
            {
                m_ModalWindow.Dispose();
                m_ModalWindow = null;
                return false;
            }
        }

        public static unsafe void EndModal()
        {
            if (m_ModalWindow == null)
            {
                throw new InvalidOperationException("BeginModal must be called before EndModal!");
            }

            m_ModalCount--;

            if (!m_ModalWindow.ShouldClose)
            {
                End();
                Window.SecondWindowEndUI(Application.Instance.Window, m_ModalWindow);
            }
            else
            {
                m_ModalWindow.Dispose();
                m_ModalWindow = null;
                m_ModalCount--;
            }

            if (m_CloseModal && m_ModalWindow != null)
            {
                m_ModalWindow.Dispose();
                m_ModalWindow = null;
                m_ModalCount--;
                m_CloseModal = false;
            }
        }

        public static unsafe void CloseCurrentModal()
        {
            if (m_ModalWindow == null)
            {
                throw new InvalidOperationException("Cannot close current modal without beginning a modal!");
            }

            m_CloseModal = true;
        }

        public static unsafe void PushFont(bool bold)
        {
            byte[] data = bold ? Application.Instance.Specification.BoldFontData : Application.Instance.Specification.DefaultFontData;
            bool result;
            fixed (byte* fontData = data)
            {
                result = _helper.ImGuiPushFont(bold, Application.Instance.Specification.DefaultFontSize, fontData, data.Length);
            }

            if (!result && Application.Instance.Specification.LoadFontOnNewSize)
            {
                throw new ArgumentException("Font size not available!");
            }
        }

        public static unsafe void PushFont(bool bold, float fontSize)
        {
            byte[] data = bold ? Application.Instance.Specification.BoldFontData : Application.Instance.Specification.DefaultFontData;
            bool result;
            fixed (byte* fontData = data)
            {
                result = _helper.ImGuiPushFont(bold, fontSize, fontData, data.Length);
            }

            if (!result && Application.Instance.Specification.LoadFontOnNewSize)
            {
                throw new ArgumentException("Font size not available!");
            }
        }

        public static unsafe void PopFont()
        {
            _imgui.PopFont();
        }

        public static unsafe bool FloatSlider(string label, ref float value, float sliderSpeed = 1.0f, float min = 0.0f, float max = 0.0f, string numFormat = "%.3f", SliderFlags flags = 0)
        {
            float* ptr = (float*)Unsafe.AsPointer(ref value);
            return _imgui.DragFloat(label, ptr, sliderSpeed, min, max, numFormat, (int)flags);
        }

        private static byte Byte(this char c)
        {
            return Encoding.UTF8.GetBytes(new[] { c })[0];
        }

        public static unsafe string OpenFile(string fileExtensionWithoutDot, string fileTypeDescription, Window owner)
        {
            /*#if WINDOWS
                        byte[] desc = Encoding.UTF8.GetBytes(fileTypeDescription);
                        byte[] ext = Encoding.UTF8.GetBytes(fileExtensionWithoutDot);
                        byte[] between = { '\0'.Byte(), '*'.Byte(), '.'.Byte() };
                        byte[] end = { '\0'.Byte() };

                        byte[] full = new byte[desc.Length + ext.Length + between.Length + end.Length];
                        desc.CopyTo(full, 0);
                        between.CopyTo(full, desc.Length);
                        ext.CopyTo(full, desc.Length + between.Length);
                        end.CopyTo(full, desc.Length + between.Length + ext.Length);
            #endif

                        byte[] buffer = new byte[260];

                        fixed (byte* ptr = buffer)
                        {
                            fixed (byte* filter = full)
                            {
                                _helper.OpenFileDialogue(filter, Application.Instance.Window.NativeHandle, (char*)ptr);
                            }
                        }

                        string result = Encoding.UTF8.GetString(buffer);

                        if (result[0] == '\0')
                            result = string.Empty;

                        return result;*/

#if WINDOWS
            using (var ofd = new System.Windows.Forms.OpenFileDialog())
            {
                System.Windows.Forms.IWin32Window? win;
                if (owner != null)
                    win = System.Windows.Forms.Control.FromHandle(owner.NativeHandle);
                else
                    win = null;

                ofd.AddExtension = true;
                ofd.Filter = $"{fileTypeDescription}|*.{fileExtensionWithoutDot}";
                ofd.RestoreDirectory = true;

                System.Windows.Forms.DialogResult result = ofd.ShowDialog(win);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FileStream fs = (FileStream)ofd.OpenFile();
                    string path = fs.Name;
                    fs.Close();
                    return path;
                }
            }

            return string.Empty;
#endif
        }

        public static unsafe string OpenFolder(string defaultPath, Window? owner = null)
        {
            /*byte[] buffer = new byte[260];

            fixed (byte* ptr = buffer)
            {
                _helper.OpenFolderDialogue(defaultPath, Application.Instance.Window.NativeHandle, ptr);
            }

            string result = Encoding.UTF8.GetString(buffer);

            if (result[0] == '\0')
                result = string.Empty;

            return result;*/

#if WINDOWS
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.IWin32Window? win;
                if (owner != null)
                    win = System.Windows.Forms.Control.FromHandle(owner.NativeHandle);
                else
                    win = null;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog(win);

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;
                }
            }

            return string.Empty;
#endif
        }

        public static string SaveFile(string fileExtensionWithoutDot, string fileTypeDescription)
        {
            return SaveFile(fileExtensionWithoutDot, fileTypeDescription, Application.Instance.Window);
        }

        public static unsafe string SaveFile(string fileExtensionWithoutDot, string fileTypeDescription, Window owner)
        {
            /*byte[] full;

#if WINDOWS
            // Combine the description and extension for windows
            byte[] desc = Encoding.UTF8.GetBytes(fileTypeDescription);
            byte[] ext = Encoding.UTF8.GetBytes(fileExtensionWithoutDot);
            byte[] between = { '\0'.Byte(), '*'.Byte(), '.'.Byte() };
            byte[] end = { '\0'.Byte() };

            full = new byte[desc.Length + ext.Length + between.Length + end.Length];
            desc.CopyTo(full, 0);
            between.CopyTo(full, desc.Length);
            ext.CopyTo(full, desc.Length + between.Length);
            end.CopyTo(full, desc.Length + between.Length + ext.Length);
#else
            // Mac OS
            full = Encoding.UTF8.GetBytes(fileExtensionWithoutDot);
#endif

            byte[] buffer = new byte[260];

            fixed (byte* ptr = buffer)
            {
                fixed (byte* filter = full)
                {
                    _helper.SaveFileDialogue(filter, window.NativeHandle, (char*)ptr);
                }
            }

            string result = Encoding.UTF8.GetString(buffer);

            if (result[0] == '\0')
                result = string.Empty;

            return result;*/

#if WINDOWS
            using (var sfd = new System.Windows.Forms.SaveFileDialog())
            {
                System.Windows.Forms.IWin32Window? win;
                if (owner != null)
                    win = System.Windows.Forms.Control.FromHandle(owner.NativeHandle);
                else
                    win = null;

                sfd.AddExtension = true;
                sfd.Filter = $"{fileTypeDescription}|*.{fileExtensionWithoutDot}";
                sfd.RestoreDirectory = true;

                System.Windows.Forms.DialogResult result = sfd.ShowDialog(win);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FileStream fs = (FileStream)sfd.OpenFile();
                    string path = fs.Name;
                    fs.Close();
                    return path;
                }
            }

            return string.Empty;
#endif
        }

        public static void InitFor(Window window)
        {
            ImGuiImpl.InitFor(
                window,
                Application.Instance.Specification.DefaultFontData,
                Application.Instance.Specification.BoldFontData,
                Application.Instance.Specification.DefaultFontSize,
                Application.Instance.Specification.DefaultFontSizes,
                Application.Instance.Specification.BoldFontSizes,
                Application.Instance.Specification.LoadFontOnNewSize
            );
        }

        // TODO: move
        public static unsafe Vector2 DisplaySize()
        {
            return *_helper.ImGuiIOGetDisplaySize();
        }
    }

    public enum UICondition
    {
        None = 0,
        Always = 1 << 0,
        Once = 1 << 1,
        FirstUseEver = 1 << 2,
        Appearing = 1 << 3,
    }

    public enum StyleVar
    {
        Alpha,               
        DisabledAlpha,       
        WindowPadding,       
        WindowRounding,      
        WindowBorderSize,    
        WindowMinSize,       
        WindowTitleAlign,    
        ChildRounding,       
        ChildBorderSize,     
        PopupRounding,       
        PopupBorderSize,     
        FramePadding,        
        FrameRounding,       
        FrameBorderSize,     
        ItemSpacing,         
        ItemInnerSpacing,    
        IndentSpacing,       
        CellPadding,         
        ScrollbarSize,       
        ScrollbarRounding,   
        GrabMinSize,         
        GrabRounding,        
        TabRounding,         
        ButtonTextAlign,     
        SelectableTextAlign, 
        SeparatorTextBorderSize,
        SeparatorTextAlign,  
        SeparatorTextPadding,
        DockingSeparatorSize,
    }

    public enum StyleColour
    {
        Text,
        TextDisabled,
        WindowBg,              
        ChildBg,               
        PopupBg,               
        Border,
        BorderShadow,
        FrameBg,               
        FrameBgHovered,
        FrameBgActive,
        TitleBg,
        TitleBgActive,
        TitleBgCollapsed,
        MenuBarBg,
        ScrollbarBg,
        ScrollbarGrab,
        ScrollbarGrabHovered,
        ScrollbarGrabActive,
        CheckMark,
        SliderGrab,
        SliderGrabActive,
        Button,
        ButtonHovered,
        ButtonActive,
        Header,                
        HeaderHovered,
        HeaderActive,
        Separator,
        SeparatorHovered,
        SeparatorActive,
        ResizeGrip,            
        ResizeGripHovered,
        ResizeGripActive,
        Tab,                   
        TabHovered,
        TabActive,
        TabUnfocused,
        TabUnfocusedActive,
        DockingPreview,        
        DockingEmptyBg,        
        PlotLines,
        PlotLinesHovered,
        PlotHistogram,
        PlotHistogramHovered,
        TableHeaderBg,         
        TableBorderStrong,     
        TableBorderLight,      
        TableRowBg,            
        TableRowBgAlt,         
        TextSelectedBg,
        DragDropTarget,        
        NavHighlight,          
        NavWindowingHighlight, 
        NavWindowingDimBg,     
        ModalWindowDimBg,      
    }

    public enum ButtonFlags
    {
        None = 0,
        MouseButtonLeft = 1 << 0,
        MouseButtonRight = 1 << 1,
        MouseButtonMiddle = 1 << 2,

        PressedOnClick = 1 << 4,
        PressedOnClickRelease = 1 << 5,
        PressedOnClickReleaseAnywhere = 1 << 6,
        PressedOnRelease = 1 << 7,
        PressedOnDoubleClick = 1 << 8,
        PressedOnDragDropHold = 1 << 9,
        Repeat = 1 << 10,
        FlattenChildren = 1 << 11,
        AllowOverlap = 1 << 12,
        DontClosePopups = 1 << 13,

        AlignTextBaseLine = 1 << 15,
        NoKeyModifiers = 1 << 16,
        NoHoldingActiveId = 1 << 17,
        NoNavFocus = 1 << 18,
        NoHoveredOnFocus = 1 << 19,
        NoSetKeyOwner = 1 << 20,
        NoTestKeyOwner = 1 << 21,
    }


    public enum WindowFlags
    {
        None = 0,
        NoTitleBar =                1 << 0,
        NoResize =                  1 << 1,
        NoMove =                    1 << 2,
        NoScrollbar =               1 << 3,
        NoScrollWithMouse =         1 << 4,
        NoCollapse =                1 << 5,
        AlwaysAutoResize =          1 << 6,
        NoBackground =              1 << 7,
        NoSavedSettings =           1 << 8,
        NoMouseInputs =             1 << 9,
        MenuBar =                   1 << 10,
        HorizontalScrollbar =       1 << 11,
        NoFocusOnAppearing =        1 << 12,
        NoBringToFrontOnFocus =     1 << 13,
        AlwaysVerticalScrollbar =   1 << 14,
        AlwaysHorizontalScrollbar = 1 << 15,
        AlwaysUseWindowPadding =    1 << 16,
        NoNavInputs =               1 << 18,
        NoNavFocus =                1 << 19,
        UnsavedDocument =           1 << 20,
        NoDocking =                 1 << 21,

        NoNav = NoNavInputs | NoNavFocus,
        NoDecoration = NoTitleBar | NoResize | NoScrollbar | NoCollapse,
        NoInputs = NoMouseInputs | NoNavInputs | NoNavFocus
    }

    public enum TableFlags
    {
        None = 0,
        Resizable = 1 << 0,
        Reorderable = 1 << 1,
        Hideable = 1 << 2,
        Sortable = 1 << 3,
        NoSavedSettings = 1 << 4,
        ContextMenuInBody = 1 << 5,
        
        RowBg = 1 << 6,
        BordersInnerH = 1 << 7,
        BordersOuterH = 1 << 8,
        BordersInnerV = 1 << 9,
        BordersOuterV = 1 << 10,
        BordersH = BordersInnerH | BordersOuterH,
        BordersV = BordersInnerV | BordersOuterV,
        BordersInner = BordersInnerV | BordersInnerH,
        BordersOuter = BordersOuterV | BordersOuterH,
        Borders = BordersInner | BordersOuter,
        NoBordersInBody = 1 << 11,
        NoBordersInBodyUntilResize = 1 << 12,
        
        SizingFixedFit = 1 << 13,
        SizingFixedSame = 2 << 13,
        SizingStretchProp = 3 << 13,
        SizingStretchSame = 4 << 13,
        
        NoHostExtendX = 1 << 16,
        NoHostExtendY = 1 << 17,
        NoKeepColumnsVisible = 1 << 18,
        PreciseWidths = 1 << 19,
        
        NoClip = 1 << 20,
        
        PadOuterX = 1 << 21,
        NoPadOuterX = 1 << 22,
        NoPadInnerX = 1 << 23,
        
        ScrollX = 1 << 24,
        ScrollY = 1 << 25,
        
        SortMulti = 1 << 26,
        SortTristate = 1 << 27
    }

    public enum TableColumnFlags
    {
        None = 0,
        Disabled = 1 << 0,
        DefaultHide = 1 << 1,
        DefaultSort = 1 << 2,
        WidthStretch = 1 << 3,
        WidthFixed = 1 << 4,
        NoResize = 1 << 5,
        NoReorder = 1 << 6,
        NoHide = 1 << 7,
        NoClip = 1 << 8,
        NoSort = 1 << 9,
        NoSortAscending = 1 << 10,
        NoSortDescending = 1 << 11,
        NoHeaderLabel = 1 << 12,
        NoHeaderWidth = 1 << 13,
        PreferSortAscending = 1 << 14,
        PreferSortDescending = 1 << 15,
        IndentEnable = 1 << 16,
        IndentDisable = 1 << 17,

        IsEnabled = 1 << 24,
        IsVisible = 1 << 25,
        IsSorted = 1 << 26,
        IsHovered = 1 << 27
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int InputTextCallback(InputTextCallbackData* data);

    public unsafe struct InputTextCallbackData
    {
        public nint Ctx;
        public InputTextFlags EventFlag;
        public InputTextFlags Flags;
        public void* UserData;
        public ushort EventChar;
        public TypedCode EventKey;
        public byte* Buf;
        public int BufTextLen;
        public int BufSize;
        public byte BufDirty;
        public int CursorPos;
        public int SelectionStart;
        public int SelectionEnd;
    }

    public enum InputTextFlags
    {
        None = 0,
        CharsDecimal = 1 << 0,
        CharsHexadecimal = 1 << 1,
        CharsUppercase = 1 << 2,
        CharsNoBlank = 1 << 3,
        AutoSelectAll = 1 << 4,
        EnterReturnsTrue = 1 << 5,
        CallbackCompletion = 1 << 6,
        CallbackHistory = 1 << 7,
        CallbackAlways = 1 << 8,
        CallbackCharFilter = 1 << 9,
        AllowTabInput = 1 << 10,
        CtrlEnterForNewLine = 1 << 11,
        NoHorizontalScroll = 1 << 12,
        AlwaysOverwrite = 1 << 13,
        ReadOnly = 1 << 14,
        Password = 1 << 15,
        NoUndoRedo = 1 << 16,
        CharsScientific = 1 << 17,
        CallbackResize = 1 << 18,
        CallbackEdit = 1 << 19,
        EscapeClearsAll = 1 << 20,
    }

    public enum SliderFlags
    {
        None = 0,
        AlwaysClamp     = 1 << 4,
        Logarithmic     = 1 << 5,
        NoRoundToFormat = 1 << 6,
        NoInput         = 1 << 7
    }

    public enum PopupFlags
    {
        None = 0,
        MouseButtonLeft = 0,        
        MouseButtonRight = 1,       
        MouseButtonMiddle = 2,      
        MouseButtonMask_ = 0x1F,
        MouseButtonDefault_ = 1,
        NoOpenOverExistingPopup = 1 << 5,  
        NoOpenOverItems = 1 << 6,
        AnyPopupId = 1 << 7,
        AnyPopupLevel = 1 << 8,  
        AnyPopup = AnyPopupId | AnyPopupLevel,
    }

    public enum TableRowFlags
    {
        None = 0,
        /// <summary>
        /// Identify header row (set default background color + width of its contents accounted differently for auto column width)
        /// </summary>
        Headers = 1 << 0,
    }

    public enum TableBgTarget
    {
        None = 0,
        /// <summary>
        /// Set row background color 0 (generally used for background, automatically set when flag RowBg is used)
        /// </summary>
        RowBg0 = 1,
        /// <summary>
        /// Set row background color 1 (generally used for selection marking)
        /// </summary>
        RowBg1 = 2,
        /// <summary>
        /// Set cell background color (top-most color)
        /// </summary>
        CellBg = 3
    }
}
