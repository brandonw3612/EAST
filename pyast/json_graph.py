from graphviz import Digraph


def module_json_to_graph(j: dict, dot: Digraph):
    label = '[Module]\n' + j['name']
    dot.node(j['id'], label, shape='box')

    body = j.get('body', [])
    with dot.subgraph() as s:
        s.attr(rank='same')
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'], n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])


def function_def_json_to_graph(j: dict, dot: Digraph):
    label = '[FunctionDef]\n' + j['name']
    if 'returns' in j:
        label += '\n(returns) ' + j['returns']
    dot.node(j['id'], label, shape='box')

    if 'arguments' in j and len(j['arguments']) > 0:
        dot.node(j['id'] + '_args', '[Args]', shape='box')
        dot.edge(j['id'], j['id'] + '_args')
        args = j.get('arguments', [])
        for n in args:
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_args', n['id'])

    dot.node(j['id'] + '_body', '[Body]', shape='box')
    dot.edge(j['id'], j['id'] + '_body')
    with dot.subgraph() as s:
        s.attr(rank='same')
        body = j.get('body', [])
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_body', n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])

def return_json_to_graph(j: dict, dot: Digraph):
    label = '[Return]'
    dot.node(j['id'], label, shape='box')

    if 'value' in j:
        json_to_graph(j['value'], dot)
        dot.edge(j['id'], j['value']['id'])

def bin_op_json_to_graph(j: dict, dot: Digraph):
    label = '[BinOp]\n' + j['op']
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['left'], dot)
    dot.edge(j['id'], j['left']['id'], label = 'left')

    json_to_graph(j['right'], dot)
    dot.edge(j['id'], j['right']['id'], label = 'right')

def assign_name_json_to_graph(j: dict, dot: Digraph):
    label = '[AssignName]\n' + j['name']
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

def name_json_to_graph(j: dict, dot: Digraph):
    label = '[Name]\n' + j['name']
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape = 'box')

def assign_json_to_graph(j: dict, dot: Digraph):
    label = '[Assign]'
    dot.node(j['id'], label, shape='box')

    dot.node(j['id'] + '_targets', '[Targets]', shape='box')
    dot.edge(j['id'], j['id'] + '_targets')
    targets = j.get('targets', [])
    for target in targets:
        json_to_graph(target, dot)
        dot.edge(j['id'] + '_targets', target['id'])

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'], label='value')

def call_json_to_graph(j: dict, dot: Digraph):
    label = '[Call]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['func'], dot)
    dot.edge(j['id'], j['func']['id'], label='func')

    if 'arguments' in j and len(j['arguments']) > 0:
        dot.node(j['id'] + '_args', '[Args]', shape='box')
        dot.edge(j['id'], j['id'] + '_args')
        args = j['arguments']
        for arg in args:
            json_to_graph(arg, dot)
            dot.edge(j['id'] + '_args', arg['id'])

def const_json_to_graph(j: dict, dot: Digraph):
    label = str(j['value']) if 'value' in j else 'None'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label)

def for_json_to_graph(j: dict, dot: Digraph):
    label = '[For]'
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['target'], dot)
    dot.edge(j['id'], j['target']['id'], label='target')

    json_to_graph(j['iter'], dot)
    dot.edge(j['id'], j['iter']['id'], label='iter')

    dot.node(j['id'] + '_body', '[Body]', shape='box')
    dot.edge(j['id'], j['id'] + '_body')

    with dot.subgraph() as s:
        s.attr(rank='same')
        body = j.get('body', [])
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_body', n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])

def attribute_json_to_graph(j: dict, dot: Digraph):
    label = '[Attribute]\n' + j['attrName']
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['expr'], dot)
    dot.edge(j['id'], j['expr']['id'], label='expr')

def list_json_to_graph(j: dict, dot: Digraph):
    label = '[List]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    if 'elements' in j and len(j['elements']) > 0:
        for e in j['elements']:
            json_to_graph(e, dot)
            dot.edge(j['id'], e['id'])

def if_json_to_graph(j: dict, dot: Digraph):
    label = '[If]'
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['condition'], dot)
    dot.edge(j['id'], j['condition']['id'], label='cond')

    dot.node(j['id'] + '_body', '[Body]', shape='box')
    dot.edge(j['id'], j['id'] + '_body')

    with dot.subgraph() as s:
        s.attr(rank='same')
        body = j.get('body', [])
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_body', n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])

    if 'orElse' in j and len(j['orElse']) > 0:
        dot.node(j['id'] + '_else', '[Else]', shape='box')
        dot.edge(j['id'], j['id'] + '_else')

        with dot.subgraph() as s:
            s.attr(rank='same')
            _else = j['orElse']
            for i in range(len(j['orElse'])):
                n = _else[i]
                json_to_graph(n, dot)
                dot.edge(j['id'] + '_else', n['id'])
                if i > 0:
                    dot.edge(_else[i - 1]['id'], n['id'], style='dashed')
                s.node(n['id'])

def compare_json_to_graph(j: dict, dot: Digraph):
    label = '[Compare]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['left'], dot)
    dot.edge(j['id'], j['left']['id'], label='left')

    dot.node(j['id'] + '_ops', '[Ops]', shape='box')
    dot.edge(j['id'], j['id'] + '_ops')

    ops = j.get('ops', [])
    for i in range(len(ops)):
        op = ops[i]
        dot.node(j['id'] + '_op_' + str(i), '[Op]\n' + op['op'], shape='box')
        dot.edge(j['id'] + '_ops', j['id'] + '_op_' + str(i))
        json_to_graph(op['value'], dot)
        dot.edge(j['id'] + '_op_' + str(i), op['value']['id'])

def expr_json_to_graph(j: dict, dot: Digraph):
    label = '[Expr]'
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'])

def unary_op_json_to_graph(j: dict, dot: Digraph):
    label = '[UnaryOp]\n' + j['op']
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['operand'], dot)
    dot.edge(j['id'], j['operand']['id'])

def break_json_to_graph(j: dict, dot: Digraph):
    dot.node(j['id'], '[Break]', shape='box')

def bool_op_json_to_graph(j: dict, dot: Digraph):
    label = '[BoolOp]\n' + j['op']
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    dot.node(j['id'] + "_values", '[Values]', shape='box')
    dot.edge(j['id'], j['id'] + "_values")

    values = j.get('values', [])
    for value in values:
        json_to_graph(value, dot)
        dot.edge(j['id'] + "_values", value['id'])

def import_json_to_graph(j: dict, dot: Digraph):
    label = '[Import]'
    dot.node(j['id'], label, shape='box')

    if 'names' in j and len(j['names']) > 0:
        dot.node(j['id'] + '_names', '[Names]', shape='box')
        dot.edge(j['id'], j['id'] + '_names')
        for name in j['names']:
            name_label = name['name']
            if 'as' in name:
                name_label += ' as ' + name['as']
            dot.node(j['id'] + '_name_' + name['name'], name_label, shape='box')
            dot.edge(j['id'] + '_names', j['id'] + '_name_' + name['name'])

def import_from_json_to_graph(j: dict, dot: Digraph):
    label = '[ImportFrom]\n' + j['module']
    if 'names' in j and len(j['names']) > 0:
        names = []
        for name in j['names']:
            name_label = name['name']
            if 'as' in name:
                name_label += ' as ' + name['as']
            names.append(name_label)
        label += '\nNames: ' + ', '.join(names)
    dot.node(j['id'], label, shape='box')

def tuple_json_to_graph(j: dict, dot: Digraph):
    label = '[Tuple]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    if 'elements' in j and len(j['elements']) > 0:
        for e in j['elements']:
            json_to_graph(e, dot)
            dot.edge(j['id'], e['id'])

def dict_json_to_graph(j: dict, dot: Digraph):
    label = '[Dict]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    if 'items' in j and len(j['items']) > 0:
        items = j['items']
        for i in range(len(items)):
            item = items[i]
            dot.node(j['id'] + '_item_' + str(i), '[Item]', shape='box')
            dot.edge(j['id'], j['id'] + '_item_' + str(i))
            json_to_graph(item['key'], dot)
            dot.edge(j['id'] + '_item_' + str(i), item['key']['id'])
            json_to_graph(item['value'], dot)
            dot.edge(j['id'] + '_item_' + str(i), item['value']['id'])

def subscript_json_to_graph(j: dict, dot: Digraph):
    label = '[Subscript]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'], label='value')

    json_to_graph(j['slice'], dot)
    dot.edge(j['id'], j['slice']['id'], label='slice')

def generator_exp_json_to_graph(j: dict, dot: Digraph):
    label = '[GeneratorExp]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['element'], dot)
    dot.edge(j['id'], j['element']['id'], label='elem')

    if 'generators' in j and len(j['generators']) > 0:
        dot.node(j['id'] + '_generators', '[Generators]', shape='box')
        dot.edge(j['id'], j['id'] + '_generators')
        for gen in j['generators']:
            json_to_graph(gen, dot)
            dot.edge(j['id'] + '_generators', gen['id'])

def comprehension_json_to_graph(j: dict, dot: Digraph):
    label = '[Comprehension]'
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['target'], dot)
    dot.edge(j['id'], j['target']['id'], label='target')

    json_to_graph(j['iter'], dot)
    dot.edge(j['id'], j['iter']['id'], label='iter')

    if 'ifs' in j and len(j['ifs']) > 0:
        dot.node(j['id'] + '_ifs', '[Ifs]', shape='box')
        dot.edge(j['id'], j['id'] + '_ifs')
        for i in j['ifs']:
            json_to_graph(i, dot)
            dot.edge(j['id'] + '_ifs', i['id'])

def aug_assign_json_to_graph(j: dict, dot: Digraph):
    label = '[AugAssign]\n' + j['op']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['target'], dot)
    dot.edge(j['id'], j['target']['id'], label='target')

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'], label='value')

def continue_json_to_graph(j: dict, dot: Digraph):
    dot.node(j['id'], '[Continue]', shape='box')

def if_exp_json_to_graph(j: dict, dot: Digraph):
    label = '[IfExp]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['condition'], dot)
    dot.edge(j['id'], j['condition']['id'], label='cond')

    json_to_graph(j['body'], dot)
    dot.edge(j['id'], j['body']['id'], label='body')

    json_to_graph(j['else'], dot)
    dot.edge(j['id'], j['else']['id'], label='else')

def while_json_to_graph(j: dict, dot: Digraph):
    label = '[While]'
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['condition'], dot)
    dot.edge(j['id'], j['condition']['id'], label='cond')

    dot.node(j['id'] + '_body', '[Body]', shape='box')
    dot.edge(j['id'], j['id'] + '_body')

    with dot.subgraph() as s:
        s.attr(rank='same')
        body = j.get('body', [])
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_body', n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])

def list_comp_json_to_graph(j: dict, dot: Digraph):
    label = '[ListComp]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['element'], dot)
    dot.edge(j['id'], j['element']['id'], label='elem')

    if 'generators' in j and len(j['generators']) > 0:
        dot.node(j['id'] + '_generators', '[Generators]', shape='box')
        dot.edge(j['id'], j['id'] + '_generators')
        for gen in j['generators']:
            json_to_graph(gen, dot)
            dot.edge(j['id'] + '_generators', gen['id'])

def joined_str_json_to_graph(j: dict, dot: Digraph):
    label = '[JoinedStr]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    if 'values' in j and len(j['values']) > 0:
        dot.node(j['id'] + '_values', '[Values]', shape='box')
        dot.edge(j['id'], j['id'] + '_values')
        for value in j['values']:
            json_to_graph(value, dot)
            dot.edge(j['id'] + '_values', value['id'])

def formatted_value_json_to_graph(j: dict, dot: Digraph):
    label = '[FormattedValue]\n' + str(j['conversion'])
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'], label='value')

    if 'format_spec' in j:
        json_to_graph(j['format_spec'], dot)
        dot.edge(j['id'], j['format_spec']['id'], label='spec')

def global_json_to_graph(j: dict, dot: Digraph):
    label = '[Global]'
    if 'names' in j and len(j['names']) > 0:
        names = j['names']
        label += '\nNames: ' + ', '.join(names)
    dot.node(j['id'], label, shape='box')

def starred_json_to_graph(j: dict, dot: Digraph):
    label = '[Starred]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'], label='value')

def slice_json_to_graph(j: dict, dot: Digraph):
    label = '[Slice]'
    dot.node(j['id'], label, shape='box')

    if 'lower' in j:
        json_to_graph(j['lower'], dot)
        dot.edge(j['id'], j['lower']['id'], label='lower')

    if 'upper' in j:
        json_to_graph(j['upper'], dot)
        dot.edge(j['id'], j['upper']['id'], label='upper')

    if 'step' in j:
        json_to_graph(j['step'], dot)
        dot.edge(j['id'], j['step']['id'], label='step')

def delete_json_to_graph(j: dict, dot: Digraph):
    label = '[Delete]'
    dot.node(j['id'], label, shape='box')

    if 'targets' in j and len(j['targets']) > 0:
        dot.node(j['id'] + '_targets', '[Targets]', shape='box')
        dot.edge(j['id'], j['id'] + '_targets')
        for target in j['targets']:
            json_to_graph(target, dot)
            dot.edge(j['id'] + '_targets', target['id'])

def set_json_to_graph(j: dict, dot: Digraph):
    label = '[Set]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    if 'elements' in j and len(j['elements']) > 0:
        for e in j['elements']:
            json_to_graph(e, dot)
            dot.edge(j['id'], e['id'])

def lambda_json_to_graph(j: dict, dot: Digraph):
    label = '[Lambda]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    dot.node(j['id'] + '_args', '[Args]', shape='box')
    dot.edge(j['id'], j['id'] + '_args')

    args = j.get('args', [])
    for arg in args:
        json_to_graph(arg, dot)
        dot.edge(j['id'] + '_args', arg['id'])

    json_to_graph(j['body'], dot)
    dot.edge(j['id'], j['body']['id'], label='body')

def dict_comp_json_to_graph(j: dict, dot: Digraph):
    label = '[DictComp]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['key'], dot)
    dot.edge(j['id'], j['key']['id'], label='key')

    json_to_graph(j['value'], dot)
    dot.edge(j['id'], j['value']['id'], label='value')

    if 'generators' in j and len(j['generators']) > 0:
        dot.node(j['id'] + '_generators', '[Generators]', shape='box')
        dot.edge(j['id'], j['id'] + '_generators')
        for gen in j['generators']:
            json_to_graph(gen, dot)
            dot.edge(j['id'] + '_generators', gen['id'])

def nonlocal_json_to_graph(j: dict, dot: Digraph):
    label = '[Nonlocal]'
    if 'names' in j and len(j['names']) > 0:
        names = j['names']
        label += '\nNames: ' + ', '.join(names)
    dot.node(j['id'], label, shape='box')

def assign_attr_json_to_graph(j: dict, dot: Digraph):
    label = '[AssignAttr]\n' + j['attrName']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['expr'], dot)
    dot.edge(j['id'], j['expr']['id'], label='expr')

def set_comp_json_to_graph(j: dict, dot: Digraph):
    label = '[SetComp]'
    if 'inferred_type' in j:
        label += '\n(i.t.) ' + j['inferred_type']
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['element'], dot)
    dot.edge(j['id'], j['element']['id'], label='elem')

    if 'generators' in j and len(j['generators']) > 0:
        dot.node(j['id'] + '_generators', '[Generators]', shape='box')
        dot.edge(j['id'], j['id'] + '_generators')
        for gen in j['generators']:
            json_to_graph(gen, dot)
            dot.edge(j['id'] + '_generators', gen['id'])

def try_json_to_graph(j: dict, dot: Digraph):
    label = '[Try]'
    dot.node(j['id'], label, shape='box')

    dot.node(j['id'] + '_body', '[Body]', shape='box')
    dot.edge(j['id'], j['id'] + '_body')
    with dot.subgraph() as s:
        s.attr(rank='same')
        body = j.get('body', [])
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_body', n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])

    if 'handlers' in j and len(j['handlers']) > 0:
        dot.node(j['id'] + '_handlers', '[Handlers]', shape='box')
        dot.edge(j['id'], j['id'] + '_handlers')
        for handler in j['handlers']:
            json_to_graph(handler, dot)
            dot.edge(j['id'] + '_handlers', handler['id'])

    if 'finally' in j and len(j['finally']) > 0:
        dot.node(j['id'] + '_finally', '[FinalBody]', shape='box')
        dot.edge(j['id'], j['id'] + '_finally')
        with dot.subgraph() as s:
            s.attr(rank='same')
            finally_block = j['finally']
            for i in range(len(finally_block)):
                n = finally_block[i]
                json_to_graph(n, dot)
                dot.edge(j['id'] + '_finally', n['id'])
                if i > 0:
                    dot.edge(finally_block[i - 1]['id'], n['id'], style='dashed')
                s.node(n['id'])

def except_handler_json_to_graph(j: dict, dot: Digraph):
    label = '[ExceptHandler]'
    dot.node(j['id'], label, shape='box')

    if 'type' in j:
        json_to_graph(j['type'], dot)
        dot.edge(j['id'], j['type']['id'], label='type')

    if 'name' in j:
        json_to_graph(j['name'], dot)
        dot.edge(j['id'], j['name']['id'], label='name')

    dot.node(j['id'] + '_body', '[Body]', shape='box')
    dot.edge(j['id'], j['id'] + '_body')
    with dot.subgraph() as s:
        s.attr(rank='same')
        body = j.get('body', [])
        for i in range(len(body)):
            n = body[i]
            json_to_graph(n, dot)
            dot.edge(j['id'] + '_body', n['id'])
            if i > 0:
                dot.edge(body[i - 1]['id'], body[i]['id'], style='dashed')
            s.node(n['id'])

def assert_json_to_graph(j: dict, dot: Digraph):
    label = '[Assert]'
    dot.node(j['id'], label, shape='box')

    json_to_graph(j['test'], dot)
    dot.edge(j['id'], j['test']['id'], label='test')

    if 'fail' in j:
        json_to_graph(j['fail'], dot)
        dot.edge(j['id'], j['fail']['id'], label='fail')

def class_def_json_to_graph(j: dict, dot: Digraph):
    label = '[ClassDef]\n' + j['name']
    dot.node(j['id'], label, shape='box')

    body = j.get('body', [])
    for i in range(len(body)):
        n = body[i]
        json_to_graph(n, dot)
        dot.edge(j['id'], n['id'])

def json_to_graph(j: dict, dot: Digraph):
    if j['node'] == 'Module':
        module_json_to_graph(j, dot)
    elif j['node'] == 'FunctionDef':
        function_def_json_to_graph(j, dot)
    elif j['node'] == 'Return':
        return_json_to_graph(j, dot)
    elif j['node'] == 'BinOp':
        bin_op_json_to_graph(j, dot)
    elif j['node'] == 'AssignName':
        assign_name_json_to_graph(j, dot)
    elif j['node'] == 'Name':
        name_json_to_graph(j, dot)
    elif j['node'] == 'Assign':
        assign_json_to_graph(j, dot)
    elif j['node'] == 'Call':
        call_json_to_graph(j, dot)
    elif j['node'] == 'Const':
        const_json_to_graph(j, dot)
    elif j['node'] == 'For':
        for_json_to_graph(j, dot)
    elif j['node'] == 'Attribute':
        attribute_json_to_graph(j, dot)
    elif j['node'] == 'List':
        list_json_to_graph(j, dot)
    elif j['node'] == 'If':
        if_json_to_graph(j, dot)
    elif j['node'] == 'Compare':
        compare_json_to_graph(j, dot)
    elif j['node'] == 'Expr':
        expr_json_to_graph(j, dot)
    elif j['node'] == 'UnaryOp':
        unary_op_json_to_graph(j, dot)
    elif j['node'] == 'Break':
        break_json_to_graph(j, dot)
    elif j['node'] == 'BoolOp':
        bool_op_json_to_graph(j, dot)
    elif j['node'] == 'Import':
        import_json_to_graph(j, dot)
    elif j['node'] == 'ImportFrom':
        import_from_json_to_graph(j, dot)
    elif j['node'] == 'Tuple':
        tuple_json_to_graph(j, dot)
    elif j['node'] == 'Dict':
        dict_json_to_graph(j, dot)
    elif j['node'] == 'Subscript':
        subscript_json_to_graph(j, dot)
    elif j['node'] == 'GeneratorExp':
        generator_exp_json_to_graph(j, dot)
    elif j['node'] == 'Comprehension':
        comprehension_json_to_graph(j, dot)
    elif j['node'] == 'AugAssign':
        aug_assign_json_to_graph(j, dot)
    elif j['node'] == 'Continue':
        continue_json_to_graph(j, dot)
    elif j['node'] == 'IfExp':
        if_exp_json_to_graph(j, dot)
    elif j['node'] == 'While':
        while_json_to_graph(j, dot)
    elif j['node'] == 'ListComp':
        list_comp_json_to_graph(j, dot)
    elif j['node'] == 'JoinedStr':
        joined_str_json_to_graph(j, dot)
    elif j['node'] == 'FormattedValue':
        formatted_value_json_to_graph(j, dot)
    elif j['node'] == 'Global':
        global_json_to_graph(j, dot)
    elif j['node'] == 'Starred':
        starred_json_to_graph(j, dot)
    elif j['node'] == 'Slice':
        slice_json_to_graph(j, dot)
    elif j['node'] == 'Delete':
        delete_json_to_graph(j, dot)
    elif j['node'] == 'Set':
        set_json_to_graph(j, dot)
    elif j['node'] == 'Lambda':
        lambda_json_to_graph(j, dot)
    elif j['node'] == 'DictComp':
        dict_comp_json_to_graph(j, dot)
    elif j['node'] == 'Nonlocal':
        nonlocal_json_to_graph(j, dot)
    elif j['node'] == 'AssignAttr':
        assign_attr_json_to_graph(j, dot)
    elif j['node'] == 'SetComp':
        set_comp_json_to_graph(j, dot)
    elif j['node'] == 'Try':
        try_json_to_graph(j, dot)
    elif j['node'] == 'ExceptHandler':
        except_handler_json_to_graph(j, dot)
    elif j['node'] == 'Assert':
        assert_json_to_graph(j, dot)
    elif j['node'] == 'ClassDef':
        class_def_json_to_graph(j, dot)

def ref_aug_graph(j: dict, node_dict: dict, dot: Digraph):
    if 'inferred_reference' in j and j['inferred_reference'] is not None and j['inferred_reference'] in node_dict:
        dot.edge(j['id'], j['inferred_reference'], 'ref', style='dashed')
    for k, v in j.items():
        if isinstance(v, dict) and 'id' in v:
            ref_aug_graph(v, node_dict, dot)
        elif isinstance(v, list):
            for item in v:
                if isinstance(item, dict) and 'id' in item:
                    ref_aug_graph(item, node_dict, dot)