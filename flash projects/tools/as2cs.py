"""Transcribes Ode to Pixel Days' decompiled ActionScript classes into C#.

The decompiler's output is very regular - one statement per line, braces on their own
lines, every member access spelled this.x - so this works line by line rather than
parsing. It does the mechanical part (declarations, types, embedded asset names, tile
data, collision callbacks); what it cannot do is reported or left for the compiler to
point at, and fixed by hand afterwards.

It was written for Ode to Pixel Days and reused for Love's First Week; each game's paths,
namespace and hand-ported callbacks are an entry in GAMES.

What needed doing by hand afterwards, for Ode: Array fields (typed lists), "for each",
[a, b] literals outside addAnimation, integer division (30 / 8), a Number assigned to a
uint, Flash-only calls (navigateToURL), and the class summaries.

Usage, from the repository root: python "flash projects/tools/as2cs.py" <game> Name [Name...]
where <game> is a key of GAMES below. It overwrites the target files.
"""
import io, os, re, sys

# One entry per game: where its decompiled classes are, where the C# goes, its namespace,
# and the collision callbacks that live in hand-ported classes (the converter finds the
# ones in the classes it is given by itself).
GAMES = {
    'ode': {
        'src': 'flash projects/OdeToPixelDays_decompiled/scripts',
        'dst': 'Assets/games/Ode to Pixel Days/Scripts',
        'namespace': 'Games.OdeToPixelDays',
        'callbacks': {
            'overlapped': ('FlxSprite', 'FlxSprite'),
            'overlappedMonsters': ('FlxSprite', 'Monster'),
            'cheerleaderOverlappedWithMachine': ('Cheerleader', 'Machine'),
            'blockParticle': ('FlxTilemap', 'BlockFalling'),
            'blockCollision': ('Hans', 'FlxSprite'),
        },
    },
    'love': {
        'src': 'flash projects/Love_s_First_Week_682x384_decompiled/scripts',
        'dst': "Assets/games/Love's First Week/Scripts",
        'namespace': 'Games.LovesFirstWeek',
        'callbacks': {},
    },
}

SRC = DST = NAMESPACE = None

TYPES = {'Number': 'double', 'Boolean': 'bool', 'String': 'string', 'int': 'int', 'uint': 'int',
         'void': 'void', 'Array': 'Array', '*': 'object', 'Class': 'string'}

CALLBACKS = {}

# Names of methods that some converted class overrides: their first declaration is virtual.
OVERRIDDEN = set()

SUMMARIES = {}


def cstype(t):
    return TYPES.get(t, t)


def find_source(name):
    for folder in ('', 'levels/'):
        path = '%s/%s%s.as' % (SRC, folder, name)
        if os.path.exists(path):
            return path, folder
    raise IOError(name)


def split_args(s):
    """Splits a parameter list on top-level commas."""
    out, depth, cur = [], 0, ''
    for ch in s:
        if ch in '([{':
            depth += 1
        elif ch in ')]}':
            depth -= 1
        if ch == ',' and depth == 0:
            out.append(cur.strip())
            cur = ''
        else:
            cur += ch
    if cur.strip():
        out.append(cur.strip())
    return out


def params(s):
    out, names = [], []
    for p in split_args(s):
        m = re.match(r'(\w+):([\w*]+)(\s*=\s*(.*))?$', p)
        name, typ, default = m.group(1), cstype(m.group(2)), m.group(4)
        names.append(name)
        out.append('%s %s%s' % (typ, name, ' = ' + default if default else ''))
    return ', '.join(out), names


def tile_rows(numbers, width, indent):
    rows = [numbers[i:i + width] for i in range(0, len(numbers), width)]
    return ',\n'.join(indent + ', '.join(row) for row in rows)


def expr(line, locals_, cls_callbacks):
    # this.x -> x, unless a local or parameter of the same name is in the way.
    line = re.sub(r'\bthis\.(\w+)', lambda m: m.group(0) if m.group(1) in locals_ else m.group(1), line)
    line = re.sub(r'\bsuper\.', 'base.', line)
    line = re.sub(r'\bfor\(var (\w+):([\w*]+) = ', lambda m: 'for(%s %s = ' % (cstype(m.group(2)), m.group(1)), line)
    line = line.replace('Math.random()', 'FlxG.random()')
    line = line.replace('Math.floor(', 'Math.Floor(').replace('Math.ceil(', 'Math.Ceiling(').replace('Math.abs(', 'Math.Abs(')
    line = line.replace("\\'", "'")
    # [0,1,2] frame lists.
    line = re.sub(r'(addAnimation\("[^"]*",)\[([\d,]*)\]', lambda m: '%snew[] { %s }' % (m.group(1), m.group(2).replace(',', ', ')), line)

    # FlxG.overlap/collide with a callback: C# cannot infer the types from a method group.
    m = re.search(r'FlxG\.(overlap|collide)\((.*)\);', line)
    if m:
        args = split_args(m.group(2))
        if len(args) == 3:
            cb = args[2].replace('this.', '')
            types = cls_callbacks.get(cb) or CALLBACKS.get(cb)
            if not types:
                print('  !! no types for callback', cb)
            else:
                line = line.replace('FlxG.%s(' % m.group(1), 'FlxG.%s<%s, %s>(' % (m.group(1), types[0], types[1]))
    return line


def spaced(line):
    """Decompiler writes f(a,b,c); the port writes f(a, b, c). Leaves strings alone."""
    out, in_str = '', False
    for i, ch in enumerate(line):
        if ch == '"' and (i == 0 or line[i - 1] != '\\'):
            in_str = not in_str
        out += ch
        if ch == ',' and not in_str and i + 1 < len(line) and line[i + 1] != ' ':
            out += ' '
    out = re.sub(r'\b(if|for|while|switch)\(', r'\1 (', out)
    return out


def convert(name):
    path, folder = find_source(name)
    src = io.open(path, encoding='utf-8').read().splitlines()
    lines = [l.strip() for l in src]

    # Callbacks this class declares.
    cls_callbacks = {}
    for l in lines:
        m = re.match(r'(?:override )?(?:public|private|protected) function (\w+)\((\w+):(\w+), (\w+):(\w+)\) : void', l)
        if m:
            cls_callbacks[m.group(1)] = (m.group(3), m.group(5))

    out = []
    base = None
    depth = 0          # brace depth inside the class
    in_class = False
    locals_ = set()
    func_depth = None
    is_ctor = False
    width = None
    i = 0
    m = re.search(r'arrayToCSV\(data,(\d+)\)', '\n'.join(lines))
    if m:
        width = int(m.group(1))
    else:
        # Love's First Week passes its levelwidth field instead of a number.
        m = re.search(r'levelwidth = (\d+);', '\n'.join(lines))
        if m:
            width = int(m.group(1))

    def emit(text):
        out.append(('\t' * (depth + 1)) + text if text else '')

    while i < len(lines):
        l = lines[i]
        i += 1
        if not in_class:
            m = re.match(r'public class (\w+)(?: extends (\w+))?', l)
            if m:
                base = m.group(2)
                in_class = True
                out.append('\tpublic class %s%s' % (m.group(1), ' : ' + base if base else ''))
                # opening brace
                i += 1
                out.append('\t{')
                depth = 1
            continue

        if l == '':
            continue

        if func_depth is None:
            # ---- member level ----
            if l == '}':
                depth -= 1
                if depth == 0:
                    out.append('\t}')
                    break
                emit('}')
                continue

            m = re.match(r'(private|public|protected) static var (\w+):Class = (\w+);', l)
            if m:
                emit('%s static string %s = "%s";' % (m.group(1), m.group(2), m.group(3)))
                # A blank line between the embedded assets and the fields, as in the hand ports.
                nxt = next((x for x in lines[i:] if x), '')
                if ' static var ' not in nxt and ' function ' not in nxt:
                    out.append('')
                continue

            m = re.match(r'(private|public|protected) (static )?(var|const) (\w+):([\w*]+)( = (.*))?;', l)
            if m:
                typ = cstype(m.group(5))
                mods = m.group(1) + (' static' if m.group(2) else '') + (' const' if m.group(3) == 'const' else '')
                emit('%s %s %s%s;' % (mods, typ, m.group(4), ' = ' + m.group(7) if m.group(7) else ''))
                continue

            m = re.match(r'(override )?(private|public|protected) function (\w+)\((.*)\)( : ([\w*]+))?$', l)
            if m:
                plist, pnames = params(m.group(4))
                is_ctor = m.group(3) == name
                locals_ = set(pnames)
                # Scan ahead for the function's locals.
                d, j = 0, i
                while j < len(lines):
                    if lines[j] == '{':
                        d += 1
                    elif lines[j] == '}':
                        d -= 1
                        if d == 0:
                            break
                    for lm in re.finditer(r'\bvar (\w+):', lines[j]):
                        locals_.add(lm.group(1))
                    j += 1
                body = lines[i + 1:j]
                if out and out[-1].strip() not in ('{', ''):
                    out.append('')
                if is_ctor:
                    if [b for b in body if b] == ['super();']:
                        # A constructor that only calls super: C# does that by itself.
                        i = j + 1
                        if out and out[-1] == '':
                            out.pop()
                        continue
                    supers = [b for b in body if re.match(r'super\(.+\);$', b)]
                    if supers:
                        base_args = expr(supers[0][len('super('):-2], locals_, cls_callbacks)
                        emit(spaced('public %s(%s) : base(%s)' % (name, plist, base_args)))
                    else:
                        emit('public %s(%s)' % (name, plist))
                else:
                    modifier = 'override ' if m.group(1) else ('virtual ' if m.group(3) in OVERRIDDEN and m.group(2) != 'private' else '')
                    emit('%s %s%s %s(%s)' % (m.group(2), modifier, cstype(m.group(6)), m.group(3), plist))
                func_depth = depth
                continue

            print('  ?? member:', l)
            continue

        # ---- inside a function ----
        if l == '{':
            emit('{')
            depth += 1
            continue
        if l == '}':
            depth -= 1
            emit('}')
            if depth == func_depth:
                func_depth = None
            else:
                nxt = next((x for x in lines[i:] if x), '')
                if nxt != '}' and not nxt.startswith('else'):
                    out.append('')
            continue

        if l == 'super();' or l.startswith('trace(') or (is_ctor and re.match(r'super\(.+\);$', l)):
            continue

        # Tile data. Long arrays are wrapped over several source lines.
        if re.match(r'(var )?data(:Array)? = new Array\(\d', l):
            while not l.endswith(');'):
                l += lines[i]
                i += 1
        m = re.match(r'(var )?data(:Array)? = new Array\(([\d,]+)\);', l)
        if m:
            numbers = m.group(3).split(',')
            emit('%sdata = new int[]' % ('int[] ' if m.group(1) else ''))
            emit('{')
            out.append(tile_rows(numbers, width or 40, '\t' * (depth + 2)))
            emit('};')
            continue
        if l == 'var data:Array = null;':
            emit('int[] data;')
            continue

        # Local declarations.
        m = re.match(r'var (\w+):([\w*]+)( = (.*))?;$', l)
        if m:
            typ, value = cstype(m.group(2)), m.group(4)
            if value is None:
                value = {'int': '0', 'double': 'double.NaN', 'bool': 'false'}.get(typ, 'null')
            if value == 'NaN':
                value = 'double.NaN'
            value = expr(value, locals_, cls_callbacks)
            if typ == 'int' and ('Math.Floor' in value or 'Math.Ceiling' in value):
                value = '(int)(%s)' % value
            emit(spaced('%s %s = %s;' % (typ, m.group(1), value)))
            continue

        emit(spaced(expr(l, locals_, cls_callbacks)))

    text = '\n'.join(out)
    uses_math = 'Math.' in text
    summary = SUMMARIES.get(name)
    if summary is None:
        summary = 'Ported from %s%s.as.' % (folder, name)
    header = ''
    if uses_math:
        header += 'using System;\n\n'
    header += 'namespace %s\n{\n' % NAMESPACE
    header += '\t/// <summary>\n' + ''.join('\t/// %s\n' % s if s else '\t///\n' for s in summary.split('\n')) + '\t/// </summary>\n'
    text = header + text + '\n}\n'

    target = '%s/%s%s.cs' % (DST, folder, name)
    io.open(target, 'w', encoding='utf-8', newline='\n').write(text)
    print(name, '->', target, '(%d lines)' % text.count('\n'))


def collect_callbacks(names):
    """Every two-argument method of the classes being converted can be a collision callback,
    and every method one of them overrides has to be virtual where it is first declared."""
    for n in names:
        path, _ = find_source(n)
        for l in io.open(path, encoding='utf-8').read().splitlines():
            m = re.match(r'\s*(?:override )?(?:public|private|protected) function (\w+)\((\w+):(\w+), (\w+):(\w+)\) : void', l)
            if m:
                CALLBACKS[m.group(1)] = (m.group(3), m.group(5))
            m = re.match(r'\s*override (?:public|protected) function (\w+)\(', l)
            if m:
                OVERRIDDEN.add(m.group(1))


if __name__ == '__main__':
    game = GAMES[sys.argv[1]]
    SRC, DST, NAMESPACE = game['src'], game['dst'], game['namespace']
    CALLBACKS.update(game['callbacks'])
    collect_callbacks(sys.argv[2:])
    for n in sys.argv[2:]:
        convert(n)
