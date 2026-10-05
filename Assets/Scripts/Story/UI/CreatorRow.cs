using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Collection.Story
{
    // One "<  Label: Value  >" row of the character creator. Left/right (arrow keys, d-pad, stick) or the arrow
    // buttons change the value; up/down moves between rows.
    public class CreatorRow : Selectable
    {
        public Text label;
        public Button left;
        public Button right;
        [Tooltip("Colour preview, shown for colour settings only.")]
        public Image swatch;

        Func<string> describe;
        Func<Color?> swatchColor;
        Action<int> change;

        public void Bind(Func<string> describe, Func<Color?> swatchColor, Action<int> change)
        {
            this.describe = describe;
            this.swatchColor = swatchColor;
            this.change = change;
            left.onClick.AddListener(() => Step(-1));
            right.onClick.AddListener(() => Step(1));
            Refresh();
        }

        public void Step(int direction)
        {
            change(direction);
            Refresh();
            // Clicking an arrow selects it; hand selection back to the row so keyboard/gamepad navigation continues.
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void Refresh()
        {
            label.text = describe();
            Color? color = swatchColor?.Invoke();
            swatch.gameObject.SetActive(color.HasValue);
            if (color.HasValue)
                swatch.color = color.Value;
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (eventData.moveDir == MoveDirection.Left)
            {
                Step(-1);
                eventData.Use();
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                Step(1);
                eventData.Use();
            }
            else
            {
                base.OnMove(eventData);
            }
        }
    }
}
