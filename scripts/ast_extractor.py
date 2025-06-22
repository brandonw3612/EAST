import ast
import ast2json
import io
import json

input_file_path = '/Users/brandon/Developer/EAST/samples/task2/task2.py'
output_file_path = '/Users/brandon/Developer/EAST/samples/task2/task2.ast.json'

with io.open(input_file_path, 'r', encoding='utf-8') as file:
    file_content = file.read()
    ast_json = ast2json.ast2json(ast.parse(file_content))
    with io.open(output_file_path, 'w', encoding='utf-8') as file:
        file.write(json.dumps(ast_json, indent=4))
        print(f"AST JSON written to {output_file_path}")