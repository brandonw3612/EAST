import astroid
import astypes


def module_to_json(module: astroid.Module, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(module)),
        'node': 'Module',
        'name': module.name,
        'body': [to_json(child, node_dict, type_inf_stats) for child in module.body]
    }
    node_dict[node['id']] = node
    return node

def function_def_to_json(fn: astroid.FunctionDef, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(fn)),
        'node': 'FunctionDef',
        'name': fn.name,
        'returns': fn.returns,
        'arguments': [to_json(arg, node_dict, type_inf_stats) for arg in fn.args.args],
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in fn.body]
    }
    node_dict[node['id']] = node
    return node

def return_to_json(rt: astroid.Return, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(rt)),
        'node': 'Return',
        'value': to_json(rt.value, node_dict, type_inf_stats) if rt.value else None
    }
    node_dict[node['id']] = node
    return node

def bin_op_to_json(op: astroid.BinOp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(op)),
        'node': 'BinOp',
        'left': to_json(op.left, node_dict, type_inf_stats),
        'op': str(op.op),
        'right': to_json(op.right, node_dict, type_inf_stats)
    }
    try:
        if 'BinOp' not in type_inf_stats:
            type_inf_stats['BinOp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['BinOp']['attempt'].append(hex(id(op)))
        inferred_type = astypes.get_type(op)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['BinOp']['success'].append(hex(id(op)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def assign_name_to_json(an: astroid.AssignName, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(an)),
        'node': 'AssignName',
        'name': an.name
    }
    try:
        if 'AssignName' not in type_inf_stats:
            type_inf_stats['AssignName'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['AssignName']['attempt'].append(hex(id(an)))
        inferred_type = astypes.get_type(an)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['AssignName']['success'].append(hex(id(an)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def name_to_json(name: astroid.Name, node_dict: dict, type_inf_stats: dict):
    _def = name.lookup(name.name)
    ref_id = hex(id(_def[1][-1])) if _def and _def[1] and (len(_def[1]) >= 1) else None
    node = {
        'id': hex(id(name)),
        'node': 'Name',
        'name': name.name,
        'inferred_reference': ref_id
    }
    try:
        if 'Name' not in type_inf_stats:
            type_inf_stats['Name'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Name']['attempt'].append(hex(id(name)))
        inferred_type = astypes.get_type(name)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Name']['success'].append(hex(id(name)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def assign_to_json(assign: astroid.Assign, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(assign)),
        'node': 'Assign',
        'targets': [to_json(target, node_dict, type_inf_stats) for target in assign.targets],
        'value': to_json(assign.value, node_dict, type_inf_stats)
    }
    node_dict[node['id']] = node
    return node

def call_to_json(call: astroid.Call, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(call)),
        'node': 'Call',
        'func': to_json(call.func, node_dict, type_inf_stats),
        'arguments': [to_json(arg, node_dict, type_inf_stats) for arg in call.args]
    }
    try:
        if 'Call' not in type_inf_stats:
            type_inf_stats['Call'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Call']['attempt'].append(hex(id(call)))
        inferred_type = astypes.get_type(call)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Call']['success'].append(hex(id(call)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def const_to_json(const: astroid.Const, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(const)),
        'node': 'Const',
        'value': const.value,
        'kind': const.kind,
        'type': astypes.get_type(const).annotation
    }
    try:
        if 'Const' not in type_inf_stats:
            type_inf_stats['Const'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Const']['attempt'].append(hex(id(const)))
        inferred_type = astypes.get_type(const)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Const']['success'].append(hex(id(const)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def for_to_json(fors: astroid.For, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(fors)),
        'node': 'For',
        'target': to_json(fors.target, node_dict, type_inf_stats),
        'iter': to_json(fors.iter, node_dict, type_inf_stats),
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in fors.body],
        'orelse': [to_json(stmt, node_dict, type_inf_stats) for stmt in fors.orelse]
    }
    node_dict[node['id']] = node
    return node

def attribute_to_json(attr: astroid.Attribute, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(attr)),
        'node': 'Attribute',
        'expr': to_json(attr.expr, node_dict, type_inf_stats),
        'attrName': attr.attrname
    }
    try:
        if 'Attribute' not in type_inf_stats:
            type_inf_stats['Attribute'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Attribute']['attempt'].append(hex(id(attr)))
        inferred_type = astypes.get_type(attr)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Attribute']['success'].append(hex(id(attr)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def list_to_json(lst: astroid.List, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(lst)),
        'node': 'List',
        'elements': [to_json(elt, node_dict, type_inf_stats) for elt in lst.elts]
    }
    try:
        if 'List' not in type_inf_stats:
            type_inf_stats['List'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['List']['attempt'].append(hex(id(lst)))
        inferred_type = astypes.get_type(lst)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['List']['success'].append(hex(id(lst)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def if_to_json(ifs: astroid.If, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(ifs)),
        'node': 'If',
        'condition': to_json(ifs.test, node_dict, type_inf_stats),
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in ifs.body],
        'orElse': [to_json(stmt, node_dict, type_inf_stats) for stmt in ifs.orelse]
    }
    node_dict[node['id']] = node
    return node

def compare_to_json(comp: astroid.Compare, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(comp)),
        'node': 'Compare',
        'left': to_json(comp.left, node_dict, type_inf_stats),
        'ops': [{ 'op': str(operator), 'value': to_json(operand, node_dict, type_inf_stats) } for (operator, operand) in comp.ops]
    }
    try:
        if 'Compare' not in type_inf_stats:
            type_inf_stats['Compare'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Compare']['attempt'].append(hex(id(comp)))
        inferred_type = astypes.get_type(comp)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Compare']['success'].append(hex(id(comp)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def expr_to_json(expr: astroid.Expr, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(expr)),
        'node': 'Expr',
        'value': to_json(expr.value, node_dict, type_inf_stats)
    }
    node_dict[node['id']] = node
    return node

def unary_op_to_json(op: astroid.UnaryOp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(op)),
        'node': 'UnaryOp',
        'op': str(op.op),
        'operand': to_json(op.operand, node_dict, type_inf_stats)
    }
    try:
        if 'UnaryOp' not in type_inf_stats:
            type_inf_stats['UnaryOp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['UnaryOp']['attempt'].append(hex(id(op)))
        inferred_type = astypes.get_type(op)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['UnaryOp']['success'].append(hex(id(op)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def break_to_json(breaks: astroid.Break, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(breaks)),
        'node': 'Break'
    }
    node_dict[node['id']] = node
    return node

def bool_op_to_json(op: astroid.BoolOp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(op)),
        'node': 'BoolOp',
        'op': str(op.op),
        'values': [to_json(value, node_dict, type_inf_stats) for value in op.values]
    }
    try:
        if 'BoolOp' not in type_inf_stats:
            type_inf_stats['BoolOp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['BoolOp']['attempt'].append(hex(id(op)))
        inferred_type = astypes.get_type(op)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['BoolOp']['success'].append(hex(id(op)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def import_to_json(imp: astroid.Import, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(imp)),
        'node': 'Import',
        'names': [{'name': name, 'as': asName} for (name, asName) in imp.names]
    }
    node_dict[node['id']] = node
    return node

def import_from_to_json(imp: astroid.ImportFrom, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(imp)),
        'node': 'ImportFrom',
        'module': imp.modname,
        'names': [{'name': name, 'as': asName} for (name, asName) in imp.names]
    }
    node_dict[node['id']] = node
    return node

def tuple_to_json(tup: astroid.Tuple, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(tup)),
        'node': 'Tuple',
        'elements': [to_json(elt, node_dict, type_inf_stats) for elt in tup.elts]
    }
    try:
        if 'Tuple' not in type_inf_stats:
            type_inf_stats['Tuple'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Tuple']['attempt'].append(hex(id(tup)))
        inferred_type = astypes.get_type(tup)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Tuple']['success'].append(hex(id(tup)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def dict_to_json(dct: astroid.Dict, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(dct)),
        'node': 'Dict',
        'items': [{'key': to_json(key, node_dict, type_inf_stats), 'value': to_json(value, node_dict, type_inf_stats)} for (key, value) in dct.items]
    }
    try:
        if 'Dict' not in type_inf_stats:
            type_inf_stats['Dict'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Dict']['attempt'].append(hex(id(dct)))
        inferred_type = astypes.get_type(dct)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Dict']['success'].append(hex(id(dct)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def subscript_to_json(sub: astroid.Subscript, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(sub)),
        'node': 'Subscript',
        'value': to_json(sub.value, node_dict, type_inf_stats),
        'slice': to_json(sub.slice, node_dict, type_inf_stats)
    }
    try:
        if 'Subscript' not in type_inf_stats:
            type_inf_stats['Subscript'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Subscript']['attempt'].append(hex(id(sub)))
        inferred_type = astypes.get_type(sub)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Subscript']['success'].append(hex(id(sub)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def generator_exp_to_json(gen: astroid.GeneratorExp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(gen)),
        'node': 'GeneratorExp',
        'element': to_json(gen.elt, node_dict, type_inf_stats),
        'generators': [to_json(gen, node_dict, type_inf_stats) for gen in gen.generators]
    }
    try:
        if 'GeneratorExp' not in type_inf_stats:
            type_inf_stats['GeneratorExp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['GeneratorExp']['attempt'].append(hex(id(gen)))
        inferred_type = astypes.get_type(gen)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['GeneratorExp']['success'].append(hex(id(gen)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def comprehension_to_json(comp: astroid.Comprehension, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(comp)),
        'node': 'Comprehension',
        'target': to_json(comp.target, node_dict, type_inf_stats),
        'iter': to_json(comp.iter, node_dict, type_inf_stats),
        'ifs': [to_json(if_, node_dict, type_inf_stats) for if_ in comp.ifs]
    }
    node_dict[node['id']] = node
    return node

def aug_assign_to_json(aug: astroid.AugAssign, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(aug)),
        'node': 'AugAssign',
        'target': to_json(aug.target, node_dict, type_inf_stats),
        'op': str(aug.op),
        'value': to_json(aug.value, node_dict, type_inf_stats)
    }
    return node

def continue_to_json(ctn: astroid.Continue, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(ctn)),
        'node': 'Continue'
    }
    node_dict[node['id']] = node
    return node

def if_exp_to_json(if_exp: astroid.IfExp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(if_exp)),
        'node': 'IfExp',
        'condition': to_json(if_exp.test, node_dict, type_inf_stats),
        'body': to_json(if_exp.body, node_dict, type_inf_stats),
        'else': to_json(if_exp.orelse, node_dict, type_inf_stats)
    }
    try:
        if 'IfExp' not in type_inf_stats:
            type_inf_stats['IfExp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['IfExp']['attempt'].append(hex(id(if_exp)))
        inferred_type = astypes.get_type(if_exp)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['IfExp']['success'].append(hex(id(if_exp)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def while_to_json(whiles: astroid.While, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(whiles)),
        'node': 'While',
        'condition': to_json(whiles.test, node_dict, type_inf_stats),
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in whiles.body],
        'orelse': [to_json(stmt, node_dict, type_inf_stats) for stmt in whiles.orelse]
    }
    node_dict[node['id']] = node
    return node

def list_comp_to_json(lst_comp: astroid.ListComp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(lst_comp)),
        'node': 'ListComp',
        'element': to_json(lst_comp.elt, node_dict, type_inf_stats),
        'generators': [to_json(gen, node_dict, type_inf_stats) for gen in lst_comp.generators]
    }
    try:
        if 'ListComp' not in type_inf_stats:
            type_inf_stats['ListComp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['ListComp']['attempt'].append(hex(id(lst_comp)))
        inferred_type = astypes.get_type(lst_comp)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['ListComp']['success'].append(hex(id(lst_comp)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def joined_str_to_json(joined_str: astroid.JoinedStr, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(joined_str)),
        'node': 'JoinedStr',
        'values': [to_json(value, node_dict, type_inf_stats) for value in joined_str.values]
    }
    try:
        if 'JoinedStr' not in type_inf_stats:
            type_inf_stats['JoinedStr'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['JoinedStr']['attempt'].append(hex(id(joined_str)))
        inferred_type = astypes.get_type(joined_str)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['JoinedStr']['success'].append(hex(id(joined_str)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def formatted_value_to_json(formatted_value: astroid.FormattedValue, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(formatted_value)),
        'node': 'FormattedValue',
        'value': to_json(formatted_value.value, node_dict, type_inf_stats),
        'conversion': formatted_value.conversion,
        'format_spec': to_json(formatted_value.format_spec, node_dict, type_inf_stats) if formatted_value.format_spec else None
    }
    try:
        if 'FormattedValue' not in type_inf_stats:
            type_inf_stats['FormattedValue'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['FormattedValue']['attempt'].append(hex(id(formatted_value)))
        inferred_type = astypes.get_type(formatted_value)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['FormattedValue']['success'].append(hex(id(formatted_value)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def global_to_json(gl: astroid.Global, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(gl)),
        'node': 'Global',
        'names': [name for name in gl.names]
    }
    node_dict[node['id']] = node
    return node

def starred_to_json(starred: astroid.Starred, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(starred)),
        'node': 'Starred',
        'value': to_json(starred.value, node_dict, type_inf_stats)
    }
    try:
        if 'Starred' not in type_inf_stats:
            type_inf_stats['Starred'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Starred']['attempt'].append(hex(id(starred)))
        inferred_type = astypes.get_type(starred)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Starred']['success'].append(hex(id(starred)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def slice_to_json(_slice: astroid.Slice, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(_slice)),
        'node': 'Slice',
        'lower': to_json(_slice.lower, node_dict, type_inf_stats) if _slice.lower else None,
        'upper': to_json(_slice.upper, node_dict, type_inf_stats) if _slice.upper else None,
        'step': to_json(_slice.step, node_dict, type_inf_stats) if _slice.step else None
    }
    node_dict[node['id']] = node
    return node

def delete_to_json(delete: astroid.Delete, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(delete)),
        'node': 'Delete',
        'targets': [to_json(target, node_dict, type_inf_stats) for target in delete.targets]
    }
    node_dict[node['id']] = node
    return node

def set_to_json(set_obj: astroid.Set, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(set_obj)),
        'node': 'Set',
        'elements': [to_json(elt, node_dict, type_inf_stats) for elt in set_obj.elts]
    }
    try:
        if 'Set' not in type_inf_stats:
            type_inf_stats['Set'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Set']['attempt'].append(hex(id(set_obj)))
        inferred_type = astypes.get_type(set_obj)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Set']['success'].append(hex(id(set_obj)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def lambda_to_json(lam: astroid.Lambda, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(lam)),
        'node': 'Lambda',
        'name': lam.name,
        'type': lam.type,
        'args': [to_json(arg, node_dict, type_inf_stats) for arg in lam.args.args],
        'body': to_json(lam.body, node_dict, type_inf_stats)
    }
    try:
        if 'Lambda' not in type_inf_stats:
            type_inf_stats['Lambda'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['Lambda']['attempt'].append(hex(id(lam)))
        inferred_type = astypes.get_type(lam)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['Lambda']['success'].append(hex(id(lam)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def dict_comp_to_json(dict_comp: astroid.DictComp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(dict_comp)),
        'node': 'DictComp',
        'key': to_json(dict_comp.key, node_dict, type_inf_stats),
        'value': to_json(dict_comp.value, node_dict, type_inf_stats),
        'generators': [to_json(gen, node_dict, type_inf_stats) for gen in dict_comp.generators]
    }
    try:
        if 'DictComp' not in type_inf_stats:
            type_inf_stats['DictComp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['DictComp']['attempt'].append(hex(id(dict_comp)))
        inferred_type = astypes.get_type(dict_comp)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['DictComp']['success'].append(hex(id(dict_comp)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def nonlocal_to_json(nonlocal_stmt: astroid.Nonlocal, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(nonlocal_stmt)),
        'node': 'Nonlocal',
        'names': [name for name in nonlocal_stmt.names]
    }
    node_dict[node['id']] = node
    return node

def assign_attr_to_json(assign_attr: astroid.AssignAttr, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(assign_attr)),
        'node': 'AssignAttr',
        'expr': to_json(assign_attr.expr, node_dict, type_inf_stats),
        'attrName': assign_attr.attrname
    }
    node_dict[node['id']] = node
    return node

def set_comp_to_json(set_comp: astroid.SetComp, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(set_comp)),
        'node': 'SetComp',
        'element': to_json(set_comp.elt, node_dict, type_inf_stats),
        'generators': [to_json(gen, node_dict, type_inf_stats) for gen in set_comp.generators]
    }
    try:
        if 'SetComp' not in type_inf_stats:
            type_inf_stats['SetComp'] = {
                'attempt': [],
                'success': []
            }
        type_inf_stats['SetComp']['attempt'].append(hex(id(set_comp)))
        inferred_type = astypes.get_type(set_comp)
        if inferred_type is not None:
            node['inferred_type'] = inferred_type.annotation
            type_inf_stats['SetComp']['success'].append(hex(id(set_comp)))
    except:
        pass
    node_dict[node['id']] = node
    return node

def try_to_json(try_stmt: astroid.Try, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(try_stmt)),
        'node': 'Try',
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in try_stmt.body],
        'handlers': [to_json(handler, node_dict, type_inf_stats) for handler in try_stmt.handlers],
        'else': [to_json(stmt, node_dict, type_inf_stats) for stmt in try_stmt.orelse],
        'finally': [to_json(stmt, node_dict, type_inf_stats) for stmt in try_stmt.finalbody]
    }
    node_dict[node['id']] = node
    return node

def except_handler_to_json(handler: astroid.ExceptHandler, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(handler)),
        'node': 'ExceptHandler',
        'type': to_json(handler.type, node_dict, type_inf_stats) if handler.type else None,
        'name': handler.name,
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in handler.body]
    }
    node_dict[node['id']] = node
    return node

def assert_to_json(assert_stmt: astroid.Assert, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(assert_stmt)),
        'node': 'Assert',
        'test': to_json(assert_stmt.test, node_dict, type_inf_stats),
        'fail': to_json(assert_stmt.fail, node_dict, type_inf_stats) if assert_stmt.fail else None
    }
    node_dict[node['id']] = node
    return node

def class_def_to_json(cls: astroid.ClassDef, node_dict: dict, type_inf_stats: dict):
    node = {
        'id': hex(id(cls)),
        'node': 'ClassDef',
        'name': cls.name,
        'body': [to_json(stmt, node_dict, type_inf_stats) for stmt in cls.body]
    }
    node_dict[node['id']] = node
    return node

def to_json(o, node_dict: dict, type_inf_stats: dict):
    if isinstance(o, astroid.Module):
        return module_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.FunctionDef):
        return function_def_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Return):
        return return_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.BinOp):
        return bin_op_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.AssignName):
        return assign_name_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Name):
        return name_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Assign):
        return assign_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Call):
        return call_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Const):
        return const_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.For):
        return for_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Attribute):
        return attribute_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.List):
        return list_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.If):
        return if_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Compare):
        return compare_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Expr):
        return expr_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.UnaryOp):
        return unary_op_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Break):
        return break_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.BoolOp):
        return bool_op_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Import):
        return import_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.ImportFrom):
        return import_from_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Tuple):
        return tuple_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Dict):
        return dict_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Subscript):
        return subscript_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.GeneratorExp):
        return generator_exp_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Comprehension):
        return comprehension_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Continue):
        return continue_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.AugAssign):
        return aug_assign_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.IfExp):
        return if_exp_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.While):
        return while_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.ListComp):
        return list_comp_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.JoinedStr):
        return joined_str_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.FormattedValue):
        return formatted_value_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Global):
        return global_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Starred):
        return starred_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Slice):
        return slice_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Delete):
        return delete_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Set):
        return set_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Lambda):
        return lambda_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.DictComp):
        return dict_comp_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Nonlocal):
        return nonlocal_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.AssignAttr):
        return assign_attr_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.SetComp):
        return set_comp_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Try):
        return try_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.ExceptHandler):
        return except_handler_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.Assert):
        return assert_to_json(o, node_dict, type_inf_stats)
    if isinstance(o, astroid.ClassDef):
        return class_def_to_json(o, node_dict, type_inf_stats)
    raise TypeError(f"Unsupported type: {type(o)}")