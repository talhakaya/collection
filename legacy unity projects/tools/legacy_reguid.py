import os,sys,re,uuid,io
# reguid.py <game folder> : every .meta GUID in the folder that also exists elsewhere under Assets gets a new GUID,
# replaced in the folder's binary files (nibble-swapped bytes) and text files.
G=sys.argv[1]
def guid_of(p):
    m=re.search(r'guid: ([0-9a-f]{32})',open(p,encoding='utf-8',errors='replace').read()); return m.group(1) if m else None
other={}
for d,_,fs in os.walk('Assets'):
    if os.path.abspath(d).startswith(os.path.abspath(G)): continue
    for f in fs:
        if f.endswith('.meta'):
            g=guid_of(os.path.join(d,f))
            if g: other[g]=os.path.join(d,f)
mine={}
for d,_,fs in os.walk(G):
    for f in fs:
        if f.endswith('.meta'):
            g=guid_of(os.path.join(d,f))
            if g in other: mine[g]=os.path.join(d,f)
sw=lambda g: bytes(int(g[i+1]+g[i],16) for i in range(0,32,2))
mp={g:uuid.uuid4().hex for g in mine}
for d,_,fs in os.walk(G):
    for f in fs:
        p=os.path.join(d,f); b=open(p,'rb').read(); b0=b
        ext=os.path.splitext(f)[1].lower()
        if f.endswith('.meta') or b[:5]==b'%YAML':
            for g,n in mp.items(): b=b.replace(g.encode(),n.encode())
        elif ext in ('.unity','.prefab','.mat','.asset','.controller','.anim','.physicmaterial','.physicsmaterial2d','.flare','.guiskin','.fontsettings'):
            for g,n in mp.items(): b=b.replace(sw(g),sw(n))
        if b!=b0: open(p,'wb').write(b)
for g,p in sorted(mine.items(), key=lambda x:x[1]): print('reguid',os.path.relpath(p,G),'(was shared with',os.path.relpath(other[g],'Assets/games')+')')
print(len(mp),'replaced')
