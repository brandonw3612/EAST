#!/usr/bin/env bash

ERROR_LOG="$(dirname "$0")/cpp_batch.log"

SAMPLES_DIR="$(dirname "$0")/samples/cpp"
ERRORS_LOG_DIR="$(dirname "$0")/samples/errors/cpp"

mkdir -p "$ERRORS_LOG_DIR"

# Traverse all directories in the samples/cpp directory
for TASK_DIR in "$SAMPLES_DIR"/*; do
  if [ -d "$TASK_DIR" ]; then
    TASK_NAME=$(basename "$TASK_DIR")
    echo "Processing task: $TASK_NAME"

    CPP_SOURCE_PATH="$TASK_DIR/$TASK_NAME.cpp"
    AST_PATH="$TASK_DIR/$TASK_NAME.ast.json"
    DOT_PATH="$TASK_DIR/$TASK_NAME.ast.dot"
    TRIMMED_DOT_PATH="$TASK_DIR/$TASK_NAME.ast.trimmed.dot"
    IMAGE_PATH="$TASK_DIR/$TASK_NAME.ast.png"
    TRIMMED_IMAGE_PATH="$TASK_DIR/$TASK_NAME.ast.trimmed.png"

    if [[ -f "$IMAGE_PATH" && -f "$TRIMMED_IMAGE_PATH" ]]; then
      echo "Results already exist, skipping task: $TASK_NAME"
      continue
    fi

    if [ -d "$ERRORS_LOG_DIR/$TASK_NAME" ]; then
      echo "Previous errors found, skipping task: $TASK_NAME"
      rm -rf "$TASK_DIR"
      continue
    fi

    if clang++ -std=c++17 -Xclang -ast-dump=json -fsyntax-only "$CPP_SOURCE_PATH" > "$AST_PATH" 2> "$ERROR_LOG"; then
      echo "Compilation passed"
    else
      rm "$AST_PATH"
      mv "$TASK_DIR" "$ERRORS_LOG_DIR/$TASK_NAME"
      mv "$ERROR_LOG" "$ERRORS_LOG_DIR/$TASK_NAME/error.log"
      echo "Compilation failed, moved to errors directory"
      continue
    fi

    if ./publish/EAST.CPP/EAST.CPP "$AST_PATH" "$DOT_PATH" "$TRIMMED_DOT_PATH" 2> "$ERROR_LOG"; then
      echo "AST processing passed"
    else
      rm "$AST_PATH"
      mv "$TASK_DIR" "$ERRORS_LOG_DIR/"
      mv "$ERROR_LOG" "$ERRORS_LOG_DIR/$TASK_NAME/error.log"
      echo "AST processing failed, moved to errors directory"
      continue
    fi

    dot -Tpng "$DOT_PATH" -o "$IMAGE_PATH"
    dot -Tpng "$TRIMMED_DOT_PATH" -o "$TRIMMED_IMAGE_PATH"

    rm "$AST_PATH" "$DOT_PATH" "$TRIMMED_DOT_PATH"
  fi
done