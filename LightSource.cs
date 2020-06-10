using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource
{

    public int radius;
    public Vec2 position;
    public Color color;

    public LightSource(int radius, Vec2 position, Color color) => (this.radius, this.position, this.color) = (radius, position, color);
    
    public static bool operator ==(LightSource l1, LightSource l2) => (l1.position.x, l1.position.y) == (l2.position.x, l2.position.y);
    
    public static bool operator != (LightSource l1, LightSource l2) => (l1.position.x, l1.position.y) != (l2.position.x, l2.position.y);

    public override bool Equals(object obj) => (obj is LightSource otherLS) ? this == otherLS : false;

}
