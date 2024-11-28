using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField]
    private Renderer m_model = null;

    [SerializeField]
    private int m_health = 100;
    [SerializeField]
    private Color m_damageColor = Color.red;

    private Color m_colorOriginal = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        m_colorOriginal = m_model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        m_health -= amount;
        StartCoroutine(DamageFlashCoroutine());

        if(m_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageFlashCoroutine()
    {
        m_model.material.color = m_damageColor;
        yield return new WaitForSeconds(.1f);
        m_model.material.color = m_colorOriginal;
    }
}
