using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    IEnumerator Start()
    {
        while(true)
        {
            spriteRenderer.sortingOrder =(int)(40 - transform.position.y);
            yield return null;
        }
    }
}
