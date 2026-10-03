import os,re,sys
sys.stdout.reconfigure(encoding='utf-8')
# closure_text.py <legacy project> scene... : GUID closure for text-serialized projects (also handles binary)
proj=sys.argv[1]; root=os.path.join(proj,'Assets')
g2p={}
for d,_,fs in os.walk(root):
    for f in fs:
        if f.endswith('.meta'):
            p=os.path.join(d,f)
            m=re.search(r'guid: ([0-9a-f]{32})',open(p,encoding='utf-8',errors='replace').read())
            if m: g2p[m.group(1)]=p[:-5]
sw=lambda g: bytes(int(g[i+1]+g[i],16) for i in range(0,32,2))
def refs(path):
    out=set()
    if os.path.isdir(path): return out
    d=open(path,'rb').read()
    if d[:5]==b'%YAML' or path.endswith('.meta') or path.endswith('.shadergraph') or path.endswith('.inputactions') or path.endswith('.json'):
        for m in re.finditer(rb'guid"?: "?([0-9a-f]{32})',d):
            g=m.group(1).decode()
            if g in g2p: out.add(g)
    else:
        for g in g2p:
            if sw(g) in d: out.add(g)
    return out
todo=[os.path.join(root,a) for a in sys.argv[2:]]
for d,_,fs in os.walk(root):
    if os.path.basename(d)=='Resources' or (os.sep+'Resources'+os.sep) in d+os.sep:
        for f in fs:
            if not f.endswith('.meta'): todo.append(os.path.join(d,f))
seen=set()
TEXT=('.unity','.prefab','.mat','.controller','.asset','.anim','.overridecontroller','.shadergraph','.shadersubgraph','.playable','.spriteatlas','.rendertexture','.physicsmaterial2d','.physicmaterial','.mask','.lighting','.inputactions','.fontsettings','.guiskin')
while todo:
    p=todo.pop()
    if p in seen or not os.path.exists(p): continue
    seen.add(p)
    rs=set()
    if os.path.splitext(p)[1].lower() in TEXT: rs|=refs(p)
    if os.path.exists(p+'.meta'): rs|=refs(p+'.meta')
    for g in rs:
        if g2p[g] not in seen: todo.append(g2p[g])
tot=0
for p in sorted(seen):
    if os.path.isfile(p): tot+=os.path.getsize(p); print(os.path.relpath(p,proj).replace(os.sep,'/'))
sys.stderr.write('files %d size %.1f MB\n'%(len(seen),tot/1e6))
