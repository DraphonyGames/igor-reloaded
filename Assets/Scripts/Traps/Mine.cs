using UnityEngine;
using System.Collections;

/// <summary>
/// this is pretty dangerous but the numbers tell you how many mines are around, ok
/// </summary>
public class Mine : MonoBehaviour
{
    /// <summary>
    /// the faction the mine belongs to. Will not damage friends.
    /// </summary>
    public CommonEntity.Faction faction = CommonEntity.Faction.NONE;
    /// <summary>
    /// played when BLOWING THE F UP
    /// </summary>
    public GameObject explosionEffectPrefab;

    /// <summary>
    /// damage dealt to anyone in range
    /// </summary>
    public float damage = 25f;

    /// <summary>
    /// sound played when exploding
    /// </summary>
    public AudioClip explosionAudioClip;

    /// <summary>
    /// Unity Callback
    /// </summary>
    /// <param name="collision">a collision</param>
    protected void OnCollisionEnter(Collision collision)
    {
        CommonEntity triggeringEntity = collision.collider.gameObject.GetComponent<CommonEntity>();
        if (!triggeringEntity || triggeringEntity.IsFriend(faction))
        {
            return;
        }

        Game.PlayAudioAtStaticPosition(explosionAudioClip, transform.position);
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // look for any unsuspecting victims in range
        // they were possibly suspecting, though
        // since there is a mine on the ground and all..

        float maxRange = 7f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxRange);
        foreach (Collider collider in colliders)
        {
            CommonEntity entity = collider.gameObject.GetComponent<CommonEntity>();
            if (!entity || entity.IsFriend(faction))
            {
                continue;
            }
            float dmg = damage * ((collider.gameObject.transform.position - transform.position).magnitude / maxRange);
            entity.TakeDamage(dmg);
        }

        Destroy(gameObject);
    }
}
