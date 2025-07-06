from datetime import datetime
from pathlib import Path

from graphviz import Digraph

from pyast.ast_json import to_json
import astroid

from pyast.json_graph import json_to_graph, ref_aug_graph


def clean_json(j):
    for key, value in list(j.items()):
        if value is None:
            del j[key]
        elif isinstance(value, dict):
            clean_json(value)
        elif isinstance(value, list):
            if len(value) == 0:
                del j[key]
            else:
                j[key] = [v for v in value if v is not None]
                for item in j[key]:
                    if isinstance(item, dict):
                        clean_json(item)
    return j

type_inf_stats = {}

i = 0
sample_dir = "./samples/python/"
# Read all subdirectories in sample_dir
for subdir in Path(sample_dir).iterdir():
    if subdir.is_dir():
        for file_path in subdir.glob("*.py"):
            i += 1
            with file_path.open("r", encoding="utf-8") as f:
                try:
                    source = f.read()
                    module = astroid.parse(source)
                    print(f'[Parsed]     {file_path} ({i}) at {datetime.now().strftime("%H:%M:%S")}')
                    node_dict = {}
                    j = clean_json(to_json(module, node_dict, type_inf_stats))
                    print(f'[Converted]  {file_path} ({i}) at {datetime.now().strftime("%H:%M:%S")}')
                    dot = Digraph()
                    json_to_graph(j, dot)
                    ref_aug_graph(j, node_dict, dot)
                    dot.render(str(file_path).replace('.py', ''), format='png', cleanup=True)
                    print(f'[Visualized] {file_path} ({i}) at {datetime.now().strftime("%H:%M:%S")}')
                except Exception as e:
                    print(f'[Error]      {file_path} ({i}) at {datetime.now().strftime("%H:%M:%S")}: {e}')
                    # print(f'[Error]      {file_path} ({i}) at {datetime.now().strftime("%H:%M:%S")}: {e.with_traceback()}')
                    continue
print()
for e in type_inf_stats:
    atmp = len(type_inf_stats[e]['attempt'])
    succ = len(type_inf_stats[e]['success'])
    print(f'{e} & {atmp} & {succ} & {atmp/i:.2f} & {succ/i:.2f} & {100*succ/atmp:.2f}')