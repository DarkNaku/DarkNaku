using UnityEngine;

namespace DarkNaku {
	public struct Size {
		public static readonly Size Zero = new Size();

		private int _width;
		private int _height;

		public int Width {
			get { return _width; }
			set { _width = value; }
		}

		public int Height {
			get { return _height; }
			set { _height = value; }
		}

		public Size(Size s) {
			_width = s.Width;
			_height = s.Height;
		}

		public Size(int width, int height) {
			_width = width;
			_height = height;
		}

		public Size(Point p) {
			_width = p.X;
			_height = p.Y;
		}

		public Size(Vector2 v) {
			_width = (int)v.x;
			_height = (int)v.y;
		}

		public Size(Vector3 v) {
			_width = (int)v.x;
			_height = (int)v.y;
		}

		public Size(int dt) {
			_width = (short)((dt >> 16) & 0xffff);
			_height = (short)(dt & 0xffff);
		}

		public static implicit operator Point(Size s) {
			return new Point(s.Width, s.Height);
		}

		public static Size operator +(Size s1, Size s2) {
			return Add(s1, s2);
		}

		public static Size operator +(Size s, Point p) {
			return Add(s, p);
		}

		public static Size operator -(Size s1, Size s2) {
			return Subtract(s1, s2);
		}

		public static Size operator -(Size s, Point p) {
			return Subtract(s, p);
		}

		public static bool operator ==(Size s1, Size s2) {
			return (s1.Width == s2.Width) && (s1.Height == s2.Height);
		}

		public static bool operator !=(Size s1, Size s2) {
			return !(s1 == s2);
		}

		public static Size Add(Size s1, Size s2) {
			return new Size(s1.Width + s2.Width, s1.Height + s2.Height);
		}

		public static Size Subtract(Size sz1, Size sz2)
		{
			return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public void Extend(int width, int height) {
			_width += width;
			_height += height;
		}

		public void Extend(Size s) {
			Extend(s.Height, s.Height);
		}

		public void Reduce(int width, int height) {
			_width -= width;
			_height -= height;
		}

		public void Reduce(Size s) {
			Reduce(s.Width, s.Height);
		}

		public override bool Equals(object obj) {
			if (!(obj is Size)) return false;
			return ((Size)obj == this);
		}

		public override int GetHashCode() {
			return Width ^ Height;
		}

		public override string ToString() {
			return "{Width=" + Width.ToString() + ", Height=" + Height.ToString() + "}";
		}
	}
}
