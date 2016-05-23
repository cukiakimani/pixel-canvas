using UnityEngine;
using System.Collections;
namespace Medvedya.SpriteDeformerTools
{
    public class HandleColorSetting
    {
        public bool inverse = false;
    }
    public class HandleColor
    {
        HandleColorSetting handleColorSetting;
        public Color standart
        {
            get
            {
                return handleColorSetting.inverse ? inverseColor(_standart) : _standart;
            }
        }
        private Color _standart = Color.white;

        public Color over
        {
            get
            {
                return handleColorSetting.inverse ? inverseColor(_over) : _over;
            }
        }
        private Color _over = Color.white;

        public Color selected
        {
            get
            {
                return handleColorSetting.inverse ? inverseColor(_selected) : _selected;
            }
        }
        private Color _selected = Color.white;


        public HandleColor(HandleColorSetting handleColorSetting, Color standart, Color? over = null, Color? selected = null)
        {
            _standart = standart;
            if (over != null) _over = (Color)over;
            if (selected != null) _selected = (Color)selected;
            this.handleColorSetting = handleColorSetting;
        }
        private Color inverseColor(Color c)
        {
           // Color newColor = c;
            c.r = 1 - c.r;
            c.g = 1 - c.g;
            c.b = 1 - c.b;
            return c;
        }
    }
}
