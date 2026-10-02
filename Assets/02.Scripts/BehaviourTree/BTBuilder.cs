using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Video;

public class BTBuilder
{
    private BTNode root;
    private readonly Stack<BTComposite> stack;

    public BTBuilder()
    {
        stack = new Stack<BTComposite>();
    }

    public BTBuilder Selector()
    {
        // 루트가 Action/Condition인 경우
        if (root != null && stack.Count == 0)
            throw new InvalidOperationException($"루트가 완성되어 노드 추가가 불가능합니다.");

        var node = new BTSelector();
        if (root == null) root = node;
        if (stack.TryPeek(out var parent)) parent.Add(node);

        stack.Push(node);
        return this;
    }

    public BTBuilder Sequence()
    {
        // 루트가 Action/Condition인 경우
        if (root != null && stack.Count == 0)
            throw new InvalidOperationException($"루트가 완성되어 노드 추가가 불가능합니다.");

        var node = new BTSequence();
        if (root == null) root = node;
        if (stack.TryPeek(out var parent)) parent.Add(node);

        stack.Push(node);
        return this;
    }

    // TODO : Action/Condition 델리게이트 전달
    public BTBuilder Action()
    {
        // 루트가 Action/Condition인 경우
        if (root != null && stack.Count == 0)
            throw new InvalidOperationException($"루트가 완성되어 노드 추가가 불가능합니다.");
        else if (root == null) root = new BTAction();
        else if (stack.TryPeek(out var parent)) parent.Add(new BTAction());

        return this;
    }
    public BTBuilder Condition()
    {
        if (root != null && stack.Count == 0) 
            throw new InvalidOperationException($"루트가 완성되어 노드 추가가 불가능합니다.");
        else if (root == null) root = new BTCondition();
        else if (stack.TryPeek(out var parent)) parent.Add(new BTCondition());

        return this;
    }

    // 윗 계층으로 이동
    public BTBuilder End()
    {
        if (stack.Count > 0) stack.Pop();
        else 
            throw new InvalidOperationException("상위 계층 노드가 없습니다.");

        return this;
    }

    // 빌드 종료 루트 반환
    public BTNode Build()
    {
        if (stack.Count > 0) 
            throw new InvalidOperationException($"End() 누락 : {stack.Count}개");
        if(root == null)
            throw new InvalidOperationException($"현재 트리의 루트가 없습니다.");

        return root;
    }
}