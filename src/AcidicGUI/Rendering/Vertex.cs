using System.Collections;
using System.Numerics;
using AcidicGUI.Effects;
using Microsoft.Xna.Framework.Graphics;

namespace AcidicGUI.Rendering;

public sealed class GuiRenderer
{
    private          GuiBatch?                       whiteBatch;
    private readonly Dictionary<Texture2D, GuiBatch> batches = new();
    private readonly IGuiContext                     context;
    private          float                             layer;
    
    public GuiRenderer(IGuiContext context)
    {
        this.context = context;
    }

    public float Layer => layer;

    public void SetLayer(float newLayer)
    {
        this.layer = newLayer;
    }
    
    public void PushLayer()
    {
        layer += 0.5f;
    }

    public void PopLayer()
    {
        layer--;
    }
    
    public void RenderGuiMesh(GuiMesh mesh)
    {
        foreach (GuiSubMesh subMesh in mesh.SubMeshes)
        {
            SubmitSubMesh(subMesh);
        }
    }

    public int GetVertexCount(Texture2D? texture)
    {
        if (texture == null)
        {
            if (whiteBatch==null)
                whiteBatch = new GuiBatch(context, GraphicsDevice, null);
            
            return whiteBatch.VertexCount;
        }

        if (!batches.TryGetValue(texture, out GuiBatch? batch))
        {
            batch = new GuiBatch(context, GraphicsDevice, texture);
            batches.Add(texture, batch);
        }

        return batch.VertexCount;
    }

    private void SubmitSubMesh(GuiSubMesh subMesh)
    {
        if (subMesh.Texture != null)
        {
            if (!batches.TryGetValue(subMesh.Texture, out GuiBatch? batch))
            {
                batch = new GuiBatch(context, GraphicsDevice, subMesh.Texture);
                batches.Add(subMesh.Texture, batch);
            }

            batch.Submit(subMesh);
            return;
        }

        if (whiteBatch==null)
            whiteBatch = new GuiBatch(context, GraphicsDevice, null);
        
        whiteBatch.Submit(subMesh);
    }

    public void RenderBatches(IEffect? effectOverride = null, float opacity = 1)
    {
        whiteBatch?.DrawBatch(effectOverride, opacity);
        
        foreach (GuiBatch batch in batches.Values)
            batch.DrawBatch(effectOverride, opacity);
    }

    public void Grab(RenderTarget2D destination)
    {
        context.Grab(destination);
    }

    public void Restore()
    {
        context.RestoreRenderState();
    }
    
    public GraphicsDevice GraphicsDevice => context.GraphicsDevice;
}

public sealed class GrowingList<T>
{
    private readonly List<T> list        = new();
    private          int     insertIndex = 0;
    
    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (T item in items)
            Add(item);
    }
    
    public void Add(T item)
    {
        if (insertIndex == list.Count)
            list.Add(item);
        else
            list[insertIndex] = item;

        insertIndex++;
    }

    public void Clear()
    {
        insertIndex = 0;
    }

    public int Count => insertIndex;

    public T[] ToArray()
    {
        return list.ToArray().AsSpan().Slice(0, insertIndex).ToArray();
    }
}

public sealed class GuiBatch
{
    private readonly GraphicsDevice                          device;
    private          VertexBuffer?                           vertexBuffer;
    private          IndexBuffer?                            indexBuffer;
    private readonly Texture2D?                              texture;
    private readonly IGuiContext                             context;
    private readonly GrowingList<VertexPositionColorTexture> cpuVertices = new();
    private readonly GrowingList<int>                        cpuIndices  = new();

    private int  vertexDirtiness = 0;
    private int  indexDirtiness  = 0;
    private bool dirty;

    public GuiBatch(IGuiContext context, GraphicsDevice device, Texture2D? texture)
    {
        this.context = context;
        this.device = device;
        this.texture = texture;
    }

    public int VertexCount => cpuVertices.Count;
    
    public void Submit(GuiSubMesh subMesh)
    {
        if (subMesh.IsNew)
        {
            subMesh.MarkAsOld();
            if (!dirty)
            {
                vertexDirtiness = cpuVertices.Count;
                indexDirtiness = cpuIndices.Count;
            }
            
            dirty = true;
        }
        
        cpuVertices.AddRange(subMesh.Vertices);
        cpuIndices.AddRange(subMesh.Indices);
    }
    
    public void DrawBatch(IEffect? effectOverride, float opacity)
    {
        if (cpuIndices.Count == 0 || cpuVertices.Count == 0)
            return;
        
        if (vertexBuffer == null || vertexBuffer.VertexCount < cpuVertices.Count)
        {
            vertexBuffer?.Dispose();
            dirty = true;
            vertexDirtiness = 0;
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColorTexture), cpuVertices.Count, BufferUsage.None);
        }
        
        if (indexBuffer == null || indexBuffer.IndexCount < cpuIndices.Count)
        {
            indexBuffer?.Dispose();
            dirty = true;
            indexDirtiness = 0;
            indexBuffer = new IndexBuffer(device, typeof(int), cpuIndices.Count, BufferUsage.None);
        }
        
        if (dirty)
        {
            var vertexData = cpuVertices.ToArray();
            var indexData = cpuIndices.ToArray();

            vertexBuffer.SetData(vertexData, vertexDirtiness, vertexData.Length - vertexDirtiness);
            indexBuffer.SetData(indexData, indexDirtiness, indexData.Length - indexDirtiness);
            
            dirty = false;
        }

        context.Render(vertexBuffer, indexBuffer, 0, cpuIndices.Count / 3, texture, null, effectOverride, opacity);
        
        cpuVertices.Clear();
        cpuIndices.Clear();
    }
}