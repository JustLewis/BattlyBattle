using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.Requests;
//using UnityEditorInternal;
using UnityEngine;

public enum BTStatus
{
    Running,
    Success,
    Failure
}


public abstract class BTNode
{

    protected MyBlackBoard bb;
    public BTNode(MyBlackBoard bbin)
    {
        this.bb = bbin;
    }

    public abstract BTStatus Execute();

    public virtual void Reset()
    {
       
    }
}

public abstract class CompositeNode : BTNode
{
    protected int CurrentChildIndex = 0;
    protected List<BTNode> Children;

    public CompositeNode(MyBlackBoard bb) : base(bb)
    {
        Children = new List<BTNode>();
    }

    public override void Reset()
    {
        CurrentChildIndex = 0;
        foreach (BTNode Child in Children)
        {
            Child.Reset();
        }
        
    }

    public void AddChild(BTNode ChildIn)
    {
        Children.Add(ChildIn);
    }
}


//iterates until child succeeds.
public class Selector : CompositeNode
{
    public Selector(MyBlackBoard bb) : base(bb)
    {

    }

    public override BTStatus Execute()
    {
        BTStatus ReturnVal = BTStatus.Failure;

        for (int i = CurrentChildIndex; i < Children.Count; i++)
        {
            ReturnVal = Children[i].Execute();
            if (ReturnVal == BTStatus.Running)
            {
                return ReturnVal;
            }
            if (ReturnVal == BTStatus.Success)
            {
                Reset();
                return ReturnVal;
            }
            if (ReturnVal == BTStatus.Failure)
            {
                CurrentChildIndex++;
                continue;
            }
        }

        Reset(); //will only get this far if all children iterated over.
        return ReturnVal;
    }
    
}

//Iterates until child fails.
public class Sequence : CompositeNode
{
    public Sequence(MyBlackBoard bb) : base(bb){ }

    public override BTStatus Execute()
    {
        BTStatus ReturnVal = BTStatus.Success;

        for (int i = CurrentChildIndex; i < Children.Count; i ++)
        {
 

            ReturnVal = Children[i].Execute();

            if(ReturnVal == BTStatus.Running)
            {
                return ReturnVal;
            }
            else if(ReturnVal == BTStatus.Failure)
            {
                Reset();
                return ReturnVal;

            }
            else if(ReturnVal == BTStatus.Success)
            {
                CurrentChildIndex ++;
                return ReturnVal;
            }
        }
        Reset();//will only get this far if all children have been executed.

        return ReturnVal;
    }

}

public abstract class DecoratorNode : BTNode 
{
    protected BTNode WrappedNode;
    public DecoratorNode(BTNode WrappedNodeIn, MyBlackBoard BBin) : base(BBin)
    {
        this.WrappedNode = WrappedNodeIn;
    }

    public BTNode GetWrappedNode()
    {
        return WrappedNode;
    }

    public override void Reset()
    {
    }
}

public class InverterDecorator : DecoratorNode
{
    public InverterDecorator(BTNode WrappedNodeIn, MyBlackBoard BBin) : base (WrappedNodeIn,BBin)
    {

    }

    public override BTStatus Execute()
    {
        BTStatus ReturnValue = WrappedNode.Execute();
        if(ReturnValue == BTStatus.Failure)
        {
            return BTStatus.Success;
        }
        else if (ReturnValue == BTStatus.Success)
        {
            return BTStatus.Failure;
        }
        return ReturnValue;
    }
}

public abstract class ConditionalDecorator : DecoratorNode
{
    public ConditionalDecorator(BTNode WrappedNode, MyBlackBoard BB) : base(WrappedNode,BB)
    {

    }

    public abstract bool CheckStatus();
    public override BTStatus Execute()
    {
        BTStatus RV = BTStatus.Failure;

        if (CheckStatus())
        {
            RV = WrappedNode.Execute();
        }
        return RV;
    }
}