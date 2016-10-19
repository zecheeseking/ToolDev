using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes_Exercise
{
    public interface ICanFly
    {
        string Fly();
    };

    public interface ICanJump
    {
        string Jump();
    };

    public interface ICanSwim
    {
        string Swim();
    };

    public interface IHasXRayVision
    {
        string SeeThroughStuff();
    };

    public interface IUseless
    {
        string Die();
    };
}