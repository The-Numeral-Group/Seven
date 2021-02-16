using UnityEngine;

/*So I (Thomas) needed a function that rotated an object to look at a specific thing.
Unity's native Quaternion.LookRotation doesn't work in three dimensions, so I needed
an alternative. Luckily, the Japanese have once again save all of technology by just
letting me copy stuff wholesale from this random blog I found. Thanks, emesiw!

Source here: http://elix-jp.sakura.ne.jp/wordpress/?p=972*/
public static class Rotations2D
{
    static public Quaternion LookRotation2D(Vector2 _vec,Vector2 _up)
    {
        float tmpAng = (Mathf.Atan2(_vec.y,_vec.x)-Mathf.Atan2(_up.y,_up.x))*Mathf.Rad2Deg;
        return Quaternion.AngleAxis(tmpAng, Vector3.forward);
    }
}
