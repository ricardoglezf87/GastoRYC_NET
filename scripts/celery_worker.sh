#!/bin/bash
cd /volume1/web/GastoRYC_NET
python3.9 -m celery -A GARCA worker -l info --pool=solo 