using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal;
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
        foreach(BTNode Child in Children)
        {
            Child.Reset();
        }
        CurrentChildIndex = 0;
    }
}

public class Selector : CompositeNode
{
    protected int ChildIterator = 0;
    public Selector(MyBlackBoard bb) : base(bb)
    {

    }

    public override BTStatus Execute()
    {
        BTStatus ReturnVal = BTStatus.Failure;

        for(int i = ChildIterator; i < Children.Count; i ++)
        {
            ReturnVal = Children[i].Execute();
            if(ReturnVal == BTStatus.Running)
            {
                ChildIterator = i;
                return ReturnVal;
            }
            if (ReturnVal == BTStatus.Success)
            {
                Reset();
                break;
            }
        }
        return ReturnVal;
    }
    
    
}

public class Sequence : CompositeNode
{
    int ChildIterator = 0;

    public Sequence(MyBlackBoard bb) : base(bb){ }

    public override BTStatus Execute()
    {
        BTStatus ReturnVal = BTStatus.Success;

        for (int i = ChildIterator;i < Children.Count; i ++)
        {
            ReturnVal = Children[i].Execute();
            if(ReturnVal == BTStatus.Running)
            {
                ChildIterator = i;
                return ReturnVal;
            }
            if(ReturnVal == BTStatus.Failure)
            {
                Reset();
                return ReturnVal;

            }
        }
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