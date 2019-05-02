using UnityEngine;

namespace DarkNaku {
	public struct Point {
		public static readonly Point Zero = new Point();

		private int _x;
		private int _y;

		public int X { 
			get { return _x; }
			set { _x = value; } 
		}
		public int Y {
			get { return _y; }
			set { _y = value; } 
		}

		public Point(Point p) {
			_x = p.X;
			_y = p.Y;
		}

		public Point(int x, int y) {
			_x = x;
			_y = y;
		}

		public Point(Size s) {
			_x = s.Width;
			_y = s.Height;
		}

		public Point(Vector2 v) {
			_x = (int)v.x;
			_y = (int)v.y;
		}

		public Point(Vector3 v) {
			_x = (int)v.x;
			_y = (int)v.y;
		}

		public Point(int dt) {
			_x = (short)((dt >> 16) & 0xffff);
			_y = (short)(dt & 0xffff);
		}

		public static implicit operator Size(Point p) {
			return new Size(p.X, p.Y);
		}

		public static implicit operator Vector2(Point p) {
			return new Vector2(p.X, p.Y);
		}

		public static implicit operator Vector3(Point p) {
			return new Vector3(p.X, p.Y, 0F);
		}

		public static Point operator +(Point p1, Point p2) {
			return Add(p1, p2);
		}

		public static Point operator +(Point p, Size s) {
			return Add(p, s);
		}

		public static Point operator -(Point p1, Point p2) {
			return Subtract(p1, p2);
		}

		public static Point operator -(Point p, Size s) {
			return Subtract(p, s);
		}

		public static bool operator ==(Point left, Point right) {
			return (left.X == right.X) && (left.Y == right.Y);
		}

		public static bool operator !=(Point left, Point right) {
			return !(left == right);
		}

		public static Point Add(Point p1, Point p2) {
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point Subtract(Point p1, Point p2) {
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		public void Offset(int dx, int dy) {
			X += dx;
			Y += dy;
		}

		public void Offset(Point p) {
			Offset(p.X, p.Y);
		}

		public override bool Equals(object obj) {
			if (!(obj is Point)) return false;
			return (Point)obj == this;
		}

		public override int GetHashCode() {
			return X ^ Y;
		}

		public override string ToString() {
			return "{X=" + X.ToString() + ",Y=" + Y.ToString() + "}";
		}
	}
}
