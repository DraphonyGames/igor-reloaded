using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// interface for all hitable objects
/// </summary>
public interface IHitable
{
    /// <summary>
    /// Deals hp damage to the object
    /// </summary>
    /// <param name="hp">damage dealt</param>
    void TakeDamage(float hp);

    /// <summary>
    /// Deals damage to the object over a period of time in a certain interval
    /// </summary>
    /// <param name="damagePerInterval">Damage dealt per interval</param>
    /// <param name="interval">Interval in which damage is dealt</param>
    /// <param name="seconds">Time until effect stops</param>
    void TakeDamageOverTime(float damagePerInterval, float interval, float seconds);
}
