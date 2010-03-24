using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace SilverUtil
{
	public class KeyHandler
	{
		private Dictionary<string, bool> isPressed;
		public KeyHandler()
		{
			BuildKeyHolder();
		}

		private void BuildKeyHolder()
		{
			isPressed = new Dictionary<string, bool>();
			object[] vals = GetValues(typeof(Key));
			foreach (object v in vals)
			{
				isPressed.Add(v.ToString(), false);
			}
		}

		public void ClearKeyPresses()
		{
			BuildKeyHolder();
		}

		public void Attach(UIElement target)
		{
			ClearKeyPresses();

			target.KeyDown += target_KeyDown;
			target.KeyUp += target_KeyUp;
			target.LostFocus += target_LostFocus;
		}

		void target_LostFocus(object sender, RoutedEventArgs e)
		{
			ClearKeyPresses();
		}

		public void Detach(UIElement target)
		{
			target.KeyDown -= target_KeyDown;
			target.KeyUp -= target_KeyUp;
			target.LostFocus -= target_LostFocus;
			ClearKeyPresses();
		}

		void target_KeyDown(object sender, KeyEventArgs e)
		{
			isPressed[e.Key.ToString()] = true;
		}

		void target_KeyUp(object sender, KeyEventArgs e)
		{
			isPressed[e.Key.ToString()] = false;
		}

		public bool IsKeyPressed(Key k)
		{
			return isPressed[k.ToString()];
		}

		private static object[] GetValues(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
			}
			List<object> values = new List<object>();

			var fields = from field in enumType.GetFields()
							 where field.IsLiteral
							 select field;

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(enumType);
				values.Add(value);
			}
			return values.ToArray();
		}
	}
}
