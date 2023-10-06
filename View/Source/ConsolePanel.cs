using Stage.Core;
using Stage.UIModule;

namespace View
{
    internal class ConsolePanel
    {
        public enum LogLevel
        {
            Info = 0,
            Warn,
            Error,
        }

        public struct ConsoleMessage
        {
            public LogLevel Level;
            public DateTime DateTime;
            public string Message;
        }

        private bool _open;
        private bool _clearOnPlay;

        private uint[] _colours =
        {
            UI.GetColourU32(new Vector4(255.0f / 255.0f, 79.0f / 255.0f, 79.0f / 255.0f, 1.0f)),
            UI.GetColourU32(new Vector4(255.0f / 255.0f, 227.0f / 255.0f, 15.0f / 255.0f, 1.0f)),
            UI.GetColourU32(new Vector4(0.0f / 255.0f, 110.0f / 255.0f, 254.0f / 255.0f, 1.0f))
        };

        private List<ConsoleMessage> _messages;

        public ConsolePanel()
        {
            _open = true;
            _clearOnPlay = false;
            _messages = new List<ConsoleMessage>();
        }

        public void OnUI()
        {
            UI.PushStyleVar(StyleVar.WindowPadding, new Vector2(0, 8));
            if (_open)
            {
                UI.Begin("Console", ref _open);

                UI.Text("");
                UI.SameLine();

                if (UI.Button("Clear"))
                {
                    Clear();
                }

                UI.SameLine();

                UI.Checkpoint("Clear On Play", ref _clearOnPlay);

                TableFlags flags = TableFlags.RowBg | TableFlags.ScrollY | TableFlags.NoPadInnerX;
                flags |= TableFlags.NoPadOuterX | TableFlags.BordersOuterV;

                UI.PushStyleColour(StyleColour.HeaderActive, new Vector4(48.0f / 255.0f, 48.0f / 255.0f, 51.0f / 255.0f, 1.0f));
                UI.PushStyleColour(StyleColour.HeaderHovered, new Vector4(48.0f / 255.0f, 48.0f / 255.0f, 51.0f / 255.0f, 1.0f));
                UI.PushStyleVar(StyleVar.ItemSpacing, Vector2.Zero);

                TableFlags headerFlags = TableFlags.ScrollY;
                headerFlags |= TableFlags.BordersInner | TableFlags.BordersInnerH | TableFlags.BordersInnerV;
                headerFlags |= TableFlags.BordersOuterV;

                UI.PushStyleVar(StyleVar.FramePadding, new Vector2(4.0f));

                if (UI.BeginTable("logTableHeader", 3, headerFlags, new Vector2(0.0f, UI.CalcTextSize("Text").Y * 2.0f)))
                {
                    UI.TableSetupColumn("Type", TableColumnFlags.WidthFixed, UI.CalcTextSize("Warning").X + 30.0f);
                    UI.TableSetupColumn("Time", TableColumnFlags.WidthFixed, UI.CalcTextSize("00:00:00").X + 30.0f);
                    UI.TableSetupColumn("Message", TableColumnFlags.WidthFixed,
                        UI.GetWindowSize().X - (UI.CalcTextSize("Warning").X + 30.0f + UI.CalcTextSize("00:00:00").X + 30.0f));

                    UI.TableNextRow(0, UI.CalcTextSize("Text").Y * 2.0f);
                    UI.TableSetBgColour(TableBgTarget.RowBg0, new Vector4(66.0f / 255.0f, 66.0f / 255.0f, 74.0f / 255.0f, 1.0f));
                    for (int column = 0; column < 3; column++)
                    {
                        UI.TableSetColumnIndex(column);
                        if (column == 0)
                        {
                            UI.AlignText(2.0f);
                            UI.Text("Type");
                        }
                        if (column == 1)
                        {
                            UI.AlignText();
                            UI.Text("Time");
                        }
                        if (column == 2)
                        {
                            UI.AlignText(-1.0f);
                            UI.Text("Message");
                        }
                    }

                    UI.EndTable();
                }

                if (UI.BeginTable("logTable", 4, flags))
                {
                    UI.TableSetupColumn("Colour", TableColumnFlags.WidthFixed | TableColumnFlags.NoHeaderWidth, 0.1f);
                    UI.TableSetupColumn("Type", TableColumnFlags.WidthFixed, UI.CalcTextSize("Warning").X + 32.0f);
                    UI.TableSetupColumn("Time", TableColumnFlags.WidthFixed, UI.CalcTextSize("00:00:00").X + 39.0f);
                    UI.TableSetupColumn("Message", TableColumnFlags.WidthFixed,
                        UI.GetWindowSize().X - (UI.CalcTextSize("Warning").X + 32.0f + UI.CalcTextSize("00:00:00").X + 39.0f));

                    foreach (ConsoleMessage msg in _messages)
                    {
                        UI.TableNextRow(0, UI.CalcTextSize("Example").Y * 2.0f);
                        for (int column = 0; column < 4; column++)
                        {
                            UI.TableSetColumnIndex(column);
                            if (column == 0)
                            {
                                uint colour = msg.Level == LogLevel.Info ? _colours[2] : msg.Level == LogLevel.Warn ? _colours[1] : _colours[0];
                                UI.TableSetBgColour(TableBgTarget.CellBg, colour);
                            }
                            if (column == 1)
                            {
                                UI.AlignText();

                                switch (msg.Level)
                                {
                                    case LogLevel.Info:
                                        UI.Text("  Info");
                                        break;
                                    case LogLevel.Warn:
                                        UI.Text("  Warning");
                                        break;
                                    case LogLevel.Error:
                                        UI.Text("  Error");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (column == 2)
                            {
                                UI.AlignText();
                                UI.Text(msg.DateTime.ToString("HH:mm:ss"));
                            }
                            if (column == 3)
                            {
                                string paddedString = "  " + msg.Message;

                                float columnWidth = UI.GetWindowSize().X - ((UI.CalcTextSize("Warning").X + 30.0f + UI.CalcTextSize("00:00:00").X + 30.0f));
                                float oneChar = UI.CalcTextSize(paddedString).X / paddedString.Length;
                                if (oneChar * paddedString.Length > columnWidth - 16.0f)
                                {
                                    float msgWidth = UI.CalcTextSize(paddedString).X;
                                    float widthOver = msgWidth - columnWidth;
                                    int charsOver = (int)(widthOver / oneChar);
                                    string newMsg = string.Empty;
                                    for (int i = 0; i < paddedString.Length - charsOver - 7; i++)
                                    {
                                        if (i < paddedString.Length - 1)
                                            newMsg += paddedString[i];
                                        else
                                            break;
                                    }
                                    newMsg += "...";
                                    UI.AlignText();
                                    UI.Text(newMsg);
                                }
                                else
                                {
                                    UI.AlignText();
                                    UI.Text(paddedString);
                                }
                            }
                        }
                    }
                    if (UI.GetScrollY() >= UI.GetScrollMaxY())
                        UI.ScrollToBottom();

                    UI.EndTable();
                }

                UI.PopStyleVar(2);
                UI.PopStyleColour(2);

                UI.End();
            }
            UI.PopStyleVar();
        }

        public void Clear()
        {
            _messages.Clear();
        }
    }
}
