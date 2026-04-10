using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BChat.Models;

namespace BChat.Custom_Controal
{
    [ToolboxItem(true)]
    [Description("حاوية ترتيب بطاقات المجموعات — تُعيد الترتيب تلقائياً عند تغيير الحجم")]
    public class GroupsWrapPanel : Panel
    {
        // ══════════════════════════════════════════════════════
        //  Fields
        // ══════════════════════════════════════════════════════
        private int _cardWidth = 280;
        private int _cardHeight = 215;
        private int _horizontalGap = 16;
        private int _verticalGap = 16;
        private int _panelPaddingH = 16;
        private int _panelPaddingV = 16;
        private bool _showAddCard = true;

        private bool _suspendLayoutUpdate = false;

        // ✅ الجديد
        private bool _rightToLeftLayoutEnabled = true;

        // ══════════════════════════════════════════════════════
        //  Events
        // ══════════════════════════════════════════════════════
        [Category("BChat - Events")]
        public event EventHandler<int> CardDeleteClicked;

        [Category("BChat - Events")]
        public event EventHandler<int> CardEditClicked;

        [Category("BChat - Events")]
        public event EventHandler<int> CardMessageClicked;

        [Category("BChat - Events")]
        public event EventHandler AddCardClicked;

        // ══════════════════════════════════════════════════════
        //  Constructor
        // ══════════════════════════════════════════════════════
        public GroupsWrapPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            UpdateStyles();

            AutoScroll = true;
            BackColor = Color.Transparent;
        }

        // ══════════════════════════════════════════════════════
        //  Properties
        // ══════════════════════════════════════════════════════

        [Category("BChat - Layout")]
        [Description("تحديد اتجاه ترتيب الكروت من اليمين إلى اليسار")]
        [DefaultValue(true)]
        public bool RightToLeftLayoutEnabled
        {
            get => _rightToLeftLayoutEnabled;
            set
            {
                _rightToLeftLayoutEnabled = value;
                RearrangeCards();
            }
        }

        public int CardWidth
        {
            get => _cardWidth;
            set { _cardWidth = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        public int CardHeight
        {
            get => _cardHeight;
            set { _cardHeight = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        public int HorizontalGap
        {
            get => _horizontalGap;
            set { _horizontalGap = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        public int VerticalGap
        {
            get => _verticalGap;
            set { _verticalGap = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        public int PanelPaddingH
        {
            get => _panelPaddingH;
            set { _panelPaddingH = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        public int PanelPaddingV
        {
            get => _panelPaddingV;
            set { _panelPaddingV = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        public bool ShowAddCard
        {
            get => _showAddCard;
            set { _showAddCard = value; if (!_suspendLayoutUpdate) RearrangeCards(); }
        }

        // ══════════════════════════════════════════════════════
        //  LoadGroups
        // ══════════════════════════════════════════════════════
        public void LoadGroups(List<CustomerGroups> groups, Func<string, Image> iconResolver = null)
        {
            SuspendLayout();
            _suspendLayoutUpdate = true;

            Controls.Clear();

            foreach (var group in groups)
                Controls.Add(BuildCard(group, iconResolver));

            if (_showAddCard)
            {
                var addCard = new AddGroupCard
                {
                    Width = _cardWidth,
                    Height = _cardHeight
                };

                addCard.AddClicked += (s, e) => AddCardClicked?.Invoke(s, e);
                Controls.Add(addCard);
            }

            _suspendLayoutUpdate = false;
            ResumeLayout(false);

            RearrangeCards();
        }

        // ══════════════════════════════════════════════════════
        //  AddGroup
        // ══════════════════════════════════════════════════════
        public void AddGroup(CustomerGroups group, Func<string, Image> iconResolver = null)
        {
            AddGroupCard addCard = null;

            foreach (Control c in Controls)
                if (c is AddGroupCard ac) { addCard = ac; break; }

            if (addCard != null)
                Controls.Remove(addCard);

            Controls.Add(BuildCard(group, iconResolver));

            if (_showAddCard && addCard != null)
                Controls.Add(addCard);

            RearrangeCards();
        }

        // ══════════════════════════════════════════════════════
        //  RemoveGroup
        // ══════════════════════════════════════════════════════
        public void RemoveGroup(int groupId)
        {
            Control target = null;

            foreach (Control c in Controls)
                if (c is CustomerGroupCard card && card.GroupId == groupId)
                {
                    target = c;
                    break;
                }

            if (target != null)
            {
                Controls.Remove(target);
                target.Dispose();
                RearrangeCards();
            }
        }

        // ══════════════════════════════════════════════════════
        //  UpdateGroup
        // ══════════════════════════════════════════════════════
        public void UpdateGroup(CustomerGroups group)
        {
            foreach (Control c in Controls)
            {
                if (c is CustomerGroupCard card && card.GroupId == group.Id)
                {
                    card.Title = group.Name ?? "";
                    card.Subtitle = group.Description ?? "";
                    card.IsActive = group.Status == statusGroups.Active;
                    card.StatOneValue = group.StatOneValue ?? "0";
                    card.StatOneLabel = group.StatOneLabel ?? "";
                    card.StatTwoValue = group.StatTwoValue ?? "-";
                    card.StatTwoLabel = group.StatTwoLabel ?? "";
                    break;
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  BuildCard
        // ══════════════════════════════════════════════════════
        private CustomerGroupCard BuildCard(CustomerGroups group, Func<string, Image> iconResolver)
        {
            var card = new CustomerGroupCard
            {
                Width = _cardWidth,
                Height = _cardHeight,
                GroupId = group.Id,
                Title = group.Name ?? "",
                Subtitle = group.Description ?? "",
                IsActive = group.Status == statusGroups.Active,
                StatOneValue = group.StatOneValue ?? "0",
                StatOneLabel = group.StatOneLabel ?? "",
                StatTwoValue = group.StatTwoValue ?? "-",
                StatTwoLabel = group.StatTwoLabel ?? "",
            };

            if (!string.IsNullOrWhiteSpace(group.IconBoxColor))
            {
                try { card.IconBoxBackColor = ColorTranslator.FromHtml(group.IconBoxColor); }
                catch { }
            }

            if (iconResolver != null && !string.IsNullOrWhiteSpace(group.Icon))
            {
                try { card.IconImage = iconResolver(group.Icon); }
                catch { }
            }

            card.DeleteClicked += (s, id) => CardDeleteClicked?.Invoke(s, id);
            card.EditClicked += (s, id) => CardEditClicked?.Invoke(s, id);
            card.MessageClicked += (s, id) => CardMessageClicked?.Invoke(s, id);

            return card;
        }

        // ══════════════════════════════════════════════════════
        //  RearrangeCards
        // ══════════════════════════════════════════════════════
        private void RearrangeCards()
        {
            if (Controls.Count == 0) return;

            int availW = Width - (_panelPaddingH * 2);
            int cols = Math.Max(1, (availW + _horizontalGap) / (_cardWidth + _horizontalGap));

            int col = 0, row = 0, maxRow = 0;

            int totalRowWidth = (cols * _cardWidth) + ((cols - 1) * _horizontalGap);

            foreach (Control ctrl in Controls)
            {
                ctrl.Width = _cardWidth;
                ctrl.Height = _cardHeight;

                if (_rightToLeftLayoutEnabled)
                {
                    // ✅ الحل الصحيح
                    ctrl.Left = Width - _panelPaddingH - ((col + 1) * _cardWidth) - (col * _horizontalGap);
                }
                else
                {
                    ctrl.Left = _panelPaddingH + col * (_cardWidth + _horizontalGap);
                }

                ctrl.Top = _panelPaddingV + row * (_cardHeight + _verticalGap);

                if (row > maxRow) maxRow = row;

                col++;

                if (col >= cols)
                {
                    col = 0;
                    row++;
                }
            }

            AutoScrollMinSize = new Size(
                0,
                _panelPaddingV + (maxRow + 1) * (_cardHeight + _verticalGap)
            );
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RearrangeCards();
        }
    }
}