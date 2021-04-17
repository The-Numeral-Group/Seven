using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndulgenceCrush : ActorAbilityFunction<Actor, int>
{
    [SerializeField]
    [Range(0.1f, 5f)]
    float jumpDuration = 0.5f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float trackDuration = 2f;
    [SerializeField]
    [Range(0.1f, 5f)]
    float fallDuration = 0.5f;
    float totalDuration;
    public GameObject shadowSpritePrefab;
    GameObject shadowSprite;

    void Awake()
    {
        totalDuration = jumpDuration + trackDuration + fallDuration;
    }

    void Start()
    {
        if (shadowSpritePrefab == null)
        {
            shadowSprite = new GameObject("Crush Shadow");
            shadowSprite.SetActive(false);
        }
        else
        {
            shadowSprite = Instantiate(shadowSpritePrefab, this.gameObject.transform);
        }
    }

    public void SetTotalAbilityDuration(float jumpD = 0.5f, float trackD = 2.5f, float fallD = 0.5f)
    {
        totalDuration = jumpDuration + trackDuration + fallDuration;
    }

    void SetupColliders(bool value)
    {
        var actorColliders = this.user.gameObject.GetComponents<Collider2D>();
        foreach(var actorCollider in actorColliders)
        {
            actorCollider.enabled = value;
        }
    }
    public override void Invoke(ref Actor user)
    {
        Debug.LogWarning("IndulgenceCrush: please use the Invoke(ref actor, param object[] args)");
    }

    public override void Invoke(ref Actor user, params object[] args)
    {
        if (isFinished)
        {
            base.Invoke(ref user, args);
        }
    }

    protected override int InternInvoke(params Actor[] args)
    {
        shadowSprite.transform.parent = user.transform;
        throw new System.NotImplementedException();
    }
}
