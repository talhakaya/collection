using UnityEngine;

namespace Games.CrimeFactory
{
	public class Rectangle {
	    public float x;
	    public float y;
	    public float w;
	    public float h;
	    public float xMin { get { return x - w * 0.5f; } }
	    public float xMax { get { return x + w * 0.5f; } }
	    public float yMin { get { return y - h * 0.5f; } }
	    public float yMax { get { return y + h * 0.5f; } }
	    private const float Atom = 0.01f;

	    public Vector2 position {
	        get {
	            return new Vector2(x, y);
	        }
	        set {
	            x = value.x;
	            y = value.y;
	        }
	    }

	    public Rectangle(float x, float y, float w, float h) {
	        this.x = x;
	        this.y = y;
	        this.w = w;
	        this.h = h;
	    }

	    public Rectangle(Vector2 position, Vector2 size) {
	        x = position.x;
	        y = position.y;
	        w = size.x;
	        h = size.y;
	    }

	    public bool Overlaps(Rectangle other, float forgive = 0f) {
	        return xMin < other.xMax - Atom - forgive && xMax > other.xMin + Atom + forgive && yMin < other.yMax - Atom - forgive && yMax > other.yMin + Atom + forgive;
	    }

	    public bool OverlapsX(Rectangle other, float forgive = 0f) {
	        return xMin < other.xMax - Atom - forgive && xMax > other.xMin + Atom + forgive;
	    }

	    public bool OverlapsY(Rectangle other, float forgive = 0f) {
	        return yMin < other.yMax - Atom - forgive && yMax > other.yMin + Atom + forgive;
	    }

	    public Rectangle Copy() {
	        return new Rectangle(x, y, w, h);
	    }
	}
}
