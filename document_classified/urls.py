# document_classified/urls.py
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import (
    documentInfoViewSet, 
    UpdateExtractedDataView,
    documentTypeViewSet,
    TestExtractionRulesView,
)

router = DefaultRouter()
router.register(r'document-types', documentTypeViewSet, basename='documenttype')
router.register(r'documents', documentInfoViewSet, basename='documentinfo') 

urlpatterns = [
    path('extracted-data/<int:document_id>/', UpdateExtractedDataView.as_view(), name='update-extracted-data'),
    path('', include(router.urls)),
    path('test-rules/', TestExtractionRulesView.as_view(), name='test-extraction-rules'),
]
