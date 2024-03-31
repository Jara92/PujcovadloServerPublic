FROM postgres
RUN chmod 700 /var/lib/postgresql/data
RUN localedef -i cs_CZ -c -f UTF-8 -A /usr/share/locale/locale.alias cs_CZ.UTF-8
ENV LANG cs_CZ.utf8