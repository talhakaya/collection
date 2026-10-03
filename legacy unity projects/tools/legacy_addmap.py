import json,collections,uuid,io,copy,sys
# addmap.py "<Map name>" spec...   spec: axis:Horizontal | axis:Vertical | axis:Name=neg/pos[,neg/pos...] | btn:Name=path[,path...]
name=sys.argv[1]
p='Assets/Resources/Input/CollectionInput.inputactions'
raw=io.open(p,encoding='utf-8',newline='').read()
bom=raw.startswith('\ufeff'); crlf='\r\n' in raw
d=json.loads(raw.lstrip('\ufeff'),object_pairs_hook=collections.OrderedDict)
otc=[m for m in d['maps'] if m['name']=='Ode to Cactus'][0]
acts={a['name']:a for a in otc['actions']}
nid=lambda: str(uuid.uuid4())
tb=[b for b in otc['bindings'] if not b['isComposite'] and not b['isPartOfComposite']][0]
SHORT={'K':'<Keyboard>/','G':'<Gamepad>/','M':'<Mouse>/'}
def full(x):
    proc=''
    if '|' in x: x,proc=x.split('|',1)
    return (SHORT[x[0]]+x[2:] if len(x)>2 and x[1]==':' and x[0] in SHORT else x),proc
def act(n,t):
    a=copy.deepcopy(acts[t]); a['name']=n; a['id']=nid(); return a
def bind(action,path,nm='',comp=False,part=False,proc=''):
    b=copy.deepcopy(tb); b['name']=nm; b['id']=nid(); b['path']=path; b['processors']=proc; b['action']=action; b['isComposite']=comp; b['isPartOfComposite']=part; return b
def axis(action,neg,pos): return [bind(action,'1DAxis','1DAxis',comp=True),bind(action,neg,'Negative',part=True),bind(action,pos,'Positive',part=True)]
dz='AxisDeadzone(min=0.19)'
m=collections.OrderedDict(); m['name']=name; m['id']=nid(); m['actions']=[]; m['bindings']=[]
for spec in sys.argv[2:]:
    kind,rest=spec.split(':',1)
    an,_,val=rest.partition('=')
    if kind=='axis':
        m['actions'].append(act(an,'Horizontal'))
        if an=='Horizontal' and not val:
            m['bindings']+=axis(an,'<Keyboard>/leftArrow','<Keyboard>/rightArrow')+axis(an,'<Keyboard>/a','<Keyboard>/d')+[bind(an,'<Gamepad>/leftStick/x',proc=dz)]+axis(an,'<Gamepad>/dpad/left','<Gamepad>/dpad/right')
        elif an=='Vertical' and not val:
            m['bindings']+=axis(an,'<Keyboard>/downArrow','<Keyboard>/upArrow')+axis(an,'<Keyboard>/s','<Keyboard>/w')+[bind(an,'<Gamepad>/leftStick/y',proc=dz)]+axis(an,'<Gamepad>/dpad/down','<Gamepad>/dpad/up')
        else:
            for pair in val.split(','):
                if '/' in pair and pair.count('~')==1:
                    n_,p_=pair.split('~'); m['bindings']+=axis(an,full(n_)[0],full(p_)[0])
                else:
                    pth,proc=full(pair); m['bindings'].append(bind(an,pth,proc=proc))
    else:
        m['actions'].append(act(an,'Jump'))
        for x in val.split(','):
            pth,proc=full(x); m['bindings'].append(bind(an,pth,proc=proc))
d['maps']=[x for x in d['maps'] if x['name']!=name]+[m]
out=json.dumps(d,indent=4,ensure_ascii=False)
if crlf: out=out.replace('\n','\r\n')
if raw.endswith('\n'): out+='\r\n' if crlf else '\n'
if bom: out='\ufeff'+out
io.open(p,'w',encoding='utf-8',newline='').write(out)
print('map',name,len(m['actions']),'actions',len(m['bindings']),'bindings')
