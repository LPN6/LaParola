#!/bin/bash

echo "What do you want to import?"
echo "1) Main db tables (import.sql)"
echo "2) Interlineare (importinterlineare.sql)"
echo "3) Quiz (importquiz.sql)"
echo "4) Riassunti (importriassunti.sql)"
echo "5) Studi (importstudi.sql)"
echo "6) Vocabolario (importvocab.sql)"
echo "ALL) All of the above"

read -p "> " choice

# Normalize choice: lowercase and trim whitespace
choice=$(echo "$choice" | tr '[:upper:]' '[:lower:]' | sed 's/^[ \t]*//;s/[ \t]*$//')

SELECTED=()
do_merge=false

case "$choice" in
  1)
    do_merge=true
    SELECTED+=("import.sql")
    ;;
  2)
    SELECTED+=("importinterlineare.sql")
    ;;
  3)
    SELECTED+=("importquiz.sql")
    ;;
  4)
    SELECTED+=("importriassunti.sql")
    ;;
  5)
    SELECTED+=("importstudi.sql")
    ;;
  6)
    SELECTED+=("importvocab.sql")
    ;;
  all)
    do_merge=true
    SELECTED+=(
      "import.sql"
      "importinterlineare.sql"
      "importquiz.sql"
      "importriassunti.sql"
      "importstudi.sql"
      "importvocab.sql"
    )
    ;;
  *)
    echo "Invalid choice: '$choice'"
    exit 1
    ;;
esac

if [ "$do_merge" = true ]; then
  docker compose exec -T db sh -c 'cd /var/lib/mysql-files/data && \
	  echo "Merging commentpulpito.txt and commentillustrator.txt..." && \
    cat commentpulpitoA.txt commentpulpitoB.txt > commentpulpito.txt && \
    cat commentillustratoreA.txt commentillustratoreB.txt > commentillustratore.txt'
fi

for f in "${SELECTED[@]}"; do
  docker compose exec -T -e f="$f" db sh -c '
    cd /var/lib/mysql-files/data &&
    echo "Importing $f..." &&
    export MYSQL_PWD=$MYSQL_PASSWORD &&
    mysql --local-infile=1 -u"$MYSQL_USER" < "$f"
  '
done
