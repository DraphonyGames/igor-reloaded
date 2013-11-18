using UnityEngine;
using System.Collections;

/// <summary>
/// needed to hit stuff
/// </summary>
public class EnemyHitUnit : MonoBehaviour
{
    /// <summary>
    /// Damage which will Make by Collision on Igor
    /// </summary>
    public float damage = 1;

    /// <summary>
    /// some effect
    /// </summary>
    public GameObject effect;

    /// <summary>
    /// flag for detecting collision
    /// </summary>
    public bool enable = false;

    /// <summary>
    /// faction this object belongs to. Will only damage other factions
    /// </summary>
    public CommonEntity.Faction faction;

    /// <summary>
    /// Unity Callback
    /// </summary>
    /// <param name="collider">a collider</param>
    private void OnTriggerEnter(Collider collider)
    {
        if (!enable)
        {
            return;
        }

        CommonEntity entity = collider.gameObject.GetComponent<CommonEntity>();
        if (entity == null || entity.IsFriend(faction))
        {
            return;
        }
        entity.TakeDamage(damage);

        Instantiate(effect, transform.position, Quaternion.AngleAxis(90, transform.right));
    }
}
