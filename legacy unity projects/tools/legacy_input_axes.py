"""Prints the axes of a legacy project's binary ProjectSettings/InputManager.asset.

    python legacy_input_axes.py "legacy unity projects/herbie/ProjectSettings/InputManager.asset"

Unity 4 saved project settings in binary, so the bindings a game was written against
cannot simply be read. The strings in the file are length-prefixed, which matters: a
plain "strings" scan drops one-letter keys (a, d, w, s, z), and with them the WASD
alternates.

For each axis this prints: name, negative/positive button, alt negative/positive
button, gravity, dead zone, sensitivity. Use it to write the game's action map in
Assets/Resources/Input/CollectionInput.inputactions (see PORTING.md).
"""
import struct
import sys


def main():
    if len(sys.argv) != 2:
        sys.exit(__doc__)
    data = open(sys.argv[1], "rb").read()

    # The type tree at the top of the file ends with the field name "joyNum"; the axis
    # data follows. Each axis starts with its name as an int32 length + bytes, padded
    # to four bytes.
    start = data.rfind(b"joyNum")
    if start < 0:
        sys.exit("No type tree found - is this a binary Unity 4 InputManager.asset?")

    def read_string(pos):
        if pos + 4 > len(data):
            return None
        (length,) = struct.unpack_from("<i", data, pos)
        if length < 0 or length > 64 or pos + 4 + length > len(data):
            return None
        raw = data[pos + 4:pos + 4 + length]
        if any(b < 0x20 or b > 0x7e for b in raw):
            return None
        return raw.decode("ascii"), pos + 4 + ((length + 3) // 4) * 4

    # Find the first plausible axis: seven strings in a row followed by three floats.
    def read_axis(pos):
        strings = []
        for _ in range(7):
            r = read_string(pos)
            if r is None:
                return None
            strings.append(r[0])
            pos = r[1]
        if pos + 12 > len(data) or not strings[0]:
            return None
        gravity, dead, sensitivity = struct.unpack_from("<fff", data, pos)
        if not (0 <= dead < 1 and 0 <= gravity < 100000 and 0 <= sensitivity < 100000):
            return None
        # snap, invert (bools, padded together), then type, axis, joyNum (int32 each).
        return strings, (gravity, dead, sensitivity), pos + 12 + 4 + 12

    pos = start
    found = None
    while pos < len(data) - 40:
        found = read_axis(pos)
        if found:
            break
        pos += 1
    if not found:
        sys.exit("No axes found.")

    print("%-14s %-12s %-12s %-12s %-12s %s" % ("name", "negative", "positive", "alt neg", "alt pos", "gravity/dead/sensitivity"))
    while found:
        s, (g, d, sens), pos = found
        print("%-14s %-12s %-12s %-12s %-12s %g / %g / %g" % (s[0], s[3], s[4], s[5], s[6], g, d, sens))
        found = read_axis(pos)


if __name__ == "__main__":
    main()
