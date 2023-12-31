using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    public static int chickens = 0; 

    [SerializeField] private TextMeshProUGUI bananasText;

    [SerializeField] private AudioSource collectionSoundEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Banana"))
        {
            collectionSoundEffect.Play();
            Destroy(collision.gameObject);
            chickens++;
            bananasText.text = "Chickens: " + chickens;
        }
    }
}
