#!/bin/sh
case "$SSH_ORIGINAL_COMMAND" in
  "show version")
    echo "MockOS v1.0, Device: device1"
    ;;
  *)
    echo "% Unknown command"
    ;;
esac