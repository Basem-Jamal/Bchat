using BChat.Custom_Controal.Custom_Bchat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BChat.Models;


namespace BChat.Custom_Controal.Custom_Bchat
{
    public class GroupSelectorPanel : Panel
    {
        private List<GroupSelectCard> _cards = new();

        public GroupSelectorPanel()
        {
            AutoScroll = true;
            Padding = new Padding(8);
            BackColor = Color.Transparent;

            Resize += (s, e) => LayoutCards();
        }

        public void LoadGroups(List<Groups> groups, Func<string, Image> iconResolver)
        {
            Controls.Clear();
            _cards.Clear();

            foreach (var g in groups)
            {
                var card = new GroupSelectCard
                {
                    GroupId = g.Id,
                    GroupName = g.Name,
                    GroupDescription = g.Description,
                    GroupIcon = iconResolver?.Invoke(g.Icon),
                    IconBoxBackColor = ColorTranslator.FromHtml(g.IconBoxColor)
                };

                _cards.Add(card);
                Controls.Add(card);
            }

            LayoutCards();
        }

        private void LayoutCards()
        {
            int y = Padding.Top;
            int width = ClientSize.Width - Padding.Horizontal - SystemInformation.VerticalScrollBarWidth;

            foreach (var card in _cards)
            {
                card.Width = width;
                card.Location = new Point(Padding.Left, y);

                y += card.Height + 8;
            }
        }

        public List<int> GetSelectedGroupIds()
        {
            return _cards
                .Where(c => c.IsSelected)
                .Select(c => c.GroupId)
                .ToList();
        }

        public void SetSelectedGroupIds(List<int> ids)
        {
            foreach (var card in _cards)
            {
                card.IsSelected = ids.Contains(card.GroupId);
            }
        }
    }

}