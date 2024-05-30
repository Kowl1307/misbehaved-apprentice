using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private GameObject heartPrefab;

    private void Start()
    {
        var instantiatedHeartCount = transform.childCount;
        var toInstantiateHearts = PlayerStats.instance.MaxHealth - instantiatedHeartCount;

        if (toInstantiateHearts > 0)
        {
            for (var i = 0; i < toInstantiateHearts; i++)
            {
                Instantiate(heartPrefab, transform);
            }
        }
        else if (toInstantiateHearts < 0)
        {
            for (var i = 0; i > toInstantiateHearts; i--)
            {
                Destroy(transform.GetChild(-i));
            }
        }
        
        //Update Visuals
        OnHealthChange(0);   
    }

    private void OnEnable()
    {
        PlayerStats.instance.onHealthChange.AddListener(OnHealthChange);
    }

    private void OnDisable()
    {
        PlayerStats.instance.onHealthChange.RemoveListener(OnHealthChange);
    }

    private void OnHealthChange(int changeAmount)
    {
        var currentHealth = PlayerStats.instance.CurrentHealth;
        var max = PlayerStats.instance.MaxHealth;
        
        //Make hearts fade into gray if the player has that max health but not current health.
        for (var i = 0; i < max; i++)
        {
            var childI = transform.GetChild(transform.childCount - 1 - i);
            childI.GetComponent<Animator>().Rebind();
            childI.GetComponent<Animator>().Update(0);
            childI.GetComponent<Animator>().enabled = i < currentHealth;
            childI.GetComponent<Image>().color = i < currentHealth ? Color.white : Color.gray;
        }
    }
}
