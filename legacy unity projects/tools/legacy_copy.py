import os,sys,shutil,re,glob
# usage: copy_closure.py <legacy folder> <dest folder> <ResourcesName> <list file> [extra rel paths...]
src,dst,resname,listf=sys.argv[1:5]
files=[l.strip() for l in open(listf,encoding='utf-8') if l.strip()]
allf=sorted(set(files+sys.argv[5:]))
have={}
for d,_,fs in os.walk('Assets'):
    for f in fs:
        if f.endswith('.meta'):
            m=re.search(r'guid: ([0-9a-f]{32})',open(os.path.join(d,f),encoding='utf-8',errors='replace').read())
            if m: have[m.group(1)]=os.path.join(d,f)
n=0;clash=0
def map_rel(rel):
    rel=rel[len('Assets/'):]
    if rel.startswith('Resources/'): rel='Resources/'+resname+'/'+rel[len('Resources/'):]
    return rel
def guid(p): return re.search(r'guid: ([0-9a-f]{32})',open(p,encoding='utf-8',errors='replace').read()).group(1)
for rel in allf:
    s=os.path.join(src,rel); t=os.path.join(dst,map_rel(rel))
    os.makedirs(os.path.dirname(t),exist_ok=True)
    if os.path.isfile(s) and not os.path.exists(t): shutil.copyfile(s,t); n+=1
    if os.path.exists(s+'.meta') and not os.path.exists(t+'.meta'):
        g=guid(s+'.meta')
        if g in have: clash+=1; print('CLASH',rel,have[g])
        shutil.copyfile(s+'.meta',t+'.meta')
    parts=rel.split('/')
    for i in range(2,len(parts)):
        folder='/'.join(parts[:i])
        sm=os.path.join(src,folder)+'.meta'; tm=os.path.join(dst,map_rel(folder))+'.meta'
        if os.path.exists(sm) and not os.path.exists(tm):
            g=guid(sm)
            if g in have: clash+=1; print('CLASH folder',folder,have[g])
            os.makedirs(os.path.dirname(tm),exist_ok=True); shutil.copyfile(sm,tm)
print('copied',n,'of',len(allf),'clashes',clash)
