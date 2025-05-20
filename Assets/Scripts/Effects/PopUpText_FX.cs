using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText_FX : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float speed;
    [SerializeField] private float disappearingSpeed;
    [SerializeField] private float colorDisappearingSpeed;
    [SerializeField] private float lifeTime;

    private float textTimer;

    void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime + Random.Range(-.3f, .3f);;
        speed = speed + Random.Range(-.5f, .5f);
        Vector3 parentScale = transform.parent.localScale;
        transform.localScale = new Vector3(1 / parentScale.x, 1 / parentScale.y, 1 / parentScale.z);
    }

    void Update()
    {  
        transform.rotation = Quaternion.identity;
        
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y +1), speed * Time.deltaTime);
        textTimer-= Time.deltaTime;

        if (textTimer < 0)
        {
            float alpha = myText.color.a - colorDisappearingSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            if (myText.color.a < 50)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y +1), disappearingSpeed * Time.deltaTime);
            
            if (myText.color.a < 0)
                Destroy(gameObject);
        }       
    }
}
