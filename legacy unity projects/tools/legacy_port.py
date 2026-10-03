import os,re,sys,io,subprocess
sys.stdout.reconfigure(encoding='utf-8')
# port.py <legacy folder> <Game Name> <new:0|1> [extra exclude regex]
legacy,name,new=sys.argv[1],sys.argv[2],sys.argv[3]=='1'
extra=sys.argv[4] if len(sys.argv)>4 else None
S=os.path.dirname(os.path.abspath(__file__))
L='legacy unity projects/'+legacy
G='Assets/games/'+name
res=re.sub(r"[^A-Za-z0-9]","",name)
EXC=r"(^|/)(UDPManager\.cs|Rotate3D\.cs|SpriteButton\.cs|PlatformerController\.cs|Thumbs\.db|.*\.pdn|.*\.suo)$|(^|/)(UnityVS|Editor|Standard Assets|Plugins|TextMesh Pro)(/|$)"
files=[]
for d,_,fs in os.walk(os.path.join(L,'Assets')):
    for f in fs:
        if f.endswith('.meta'): continue
        rel=os.path.relpath(os.path.join(d,f),L).replace(os.sep,'/')
        if re.search(EXC,rel) or (extra and re.search(extra,rel)): continue
        files.append(rel)
io.open(os.path.join(S,'list.txt'),'w',encoding='utf-8').write('\n'.join(files)+'\n')
def run(*a):
    r=subprocess.run(list(a),capture_output=True,text=True,encoding='utf-8',errors='replace'); return (r.stdout+r.stderr).strip()
print(run('python',os.path.join(S,'legacy_copy.py'),L,G,res,os.path.join(S,'list.txt')).split('\n')[-1])
print(run('python','legacy unity projects/tools/legacy_reguid.py',G).split('\n')[-1])
out=run('python','legacy unity projects/tools/legacy_scripts.py',G,re.sub(r"[^A-Za-z0-9 ]","",name))
print('\n'.join(l for l in out.split('\n') if 'BY HAND' in l or 'declared' in l)[:1500])
ESC=re.compile(r'[ \t]*if \((?:Input|TaloketoInputManager)\.GetKey(?:Down)?\(KeyCode\.Escape\)\)\s*\{\s*Application\.Quit\s*\(\);\s*\}[ \t]*\r?\n')
for d,_,fs in os.walk(G):
    for f in fs:
        if not f.endswith('.cs'): continue
        p=os.path.join(d,f); s=io.open(p,encoding='utf-8',newline='').read(); s0=s
        s=ESC.sub('\t\t\t// In the collection: the Escape-quit is gone (the collection has its own exit).\n',s)
        if new: s=re.sub(r'([A-Za-z_][A-Za-z0-9_]*)\.GetComponent<Collider>\(\)',r'\1.collider',s)
        if f=='TalhaAnimation.cs':
            m=re.search(r"\n(\s*)i = Mathf.FloorToInt\(.*\n",s)
            if m: s=s.replace(m.group(0),m.group(0)+m.group(1)+"// In the collection: the float maths can land on sprites.Length, so it is clamped.\n"+m.group(1)+"i = Mathf.Clamp(i, 0, sprites.Length - 1);\n")
        if f=='TalhaColorChanger.cs':
            a="i = Mathf.FloorToInt((Game.time % (period * colors.Length)) / period);"
            if a in s: s=s.replace(a,"// In the collection: the float maths can land on colors.Length, so it is clamped.\n\t\t\t\ti = Mathf.Min(Mathf.FloorToInt((Game.time % (period * colors.Length)) / period), colors.Length - 1);")
        for a in ('Screen.showCursor = false;','Cursor.visible = false;'):
            if a in s:
                s=s.replace(a,'Collection.Controls.GlobalInputManager.HideGameCursor(); // In the collection: was '+a[:-1])
        if s!=s0: io.open(p,'w',encoding='utf-8',newline='').write(s)
print('files',len(files),'size %.1f MB'%(sum(os.path.getsize(os.path.join(L,f)) for f in files)/1e6))
pat=re.compile(r'(?<![A-Za-z])Input\.[A-Za-z]+\(?[^;]{0,40}|Application\.(Quit|LoadLevel|loadedLevel)[^;]{0,30}|Resources\.Load[^;]{0,50}|PlayerPrefs\.[A-Za-z]+\("[^"]*|void OnMouse[A-Za-z]+|void OnGUI|DontDestroyOnLoad|Shader\.Find[^;]{0,40}|\.contacts\[|WebCam|Microphone|LoadScene\([^;]{0,40}|CompareTag\("[^"]*|NameToLayer\("[^"]*')
print('--- left to do:')
for d,_,fs in os.walk(G):
    for f in sorted(fs):
        if not f.endswith('.cs'): continue
        for i,l in enumerate(io.open(os.path.join(d,f),encoding='utf-8').read().split('\n')):
            if l.strip().startswith('//'): continue
            for m in pat.finditer(l):
                if 'TaloketoInputManager' in l and m.group(0).startswith('Input.'): continue
                print('  %s:%d: %s'%(f,i+1,m.group(0)[:90]))
