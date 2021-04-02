using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public BoxCollider2D chestCollider;
    public void Update()
    {
        if (Time.timeScale == 0)
            OnChestCollisionEnter2D(chestCollider);
    }
    // works perfect while game is paused
    private void OnChestCollisionEnter2D(BoxCollider2D col)
    {
        Collider2D boxCollider = Physics2D.OverlapBox(col.transform.position, new Vector2(col.size.x, col.size.y), 0f);
        if(boxCollider != null)
            if (boxCollider.name == "Circle")
                boxCollider.gameObject.SetActive(false);
    }
    // works perfect while game is running
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Circle")
        {
            // hide circle with animation
            collision.gameObject.SetActive(false);
            //col.collider.GetComponent<Animator>().SetTrigger("Dying");
        }
    }
}
