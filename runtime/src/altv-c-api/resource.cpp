//
// Created by Fabian Terhorst on 2019-02-23.
//

#include "resource.h"

uint64_t Resource_GetExportsCount(alt::IResource* resource) {
    alt::IMValueDict* dict = resource->GetExports().Get();
    if (dict == nullptr) return 0;
    return dict->GetSize();
}

void Resource_GetExports(alt::IResource* resource, const char* keys[],
                         alt::MValueConst* values[]) {
    auto dict = resource->GetExports();
    if (dict.Get() == nullptr) {
        return;
    }
    auto next = dict->Begin();
    uint64_t i = 0;
    do {
        alt::MValueConst mValueElement = next->GetValue();
        keys[i] = next->GetKey().CStr();
        values[i] = &mValueElement;
    } while ((next = dict->Next()) != nullptr);
}

alt::MValueConst* Resource_GetExport(alt::IResource* resource, const char* key) {
    alt::IMValueDict* dict = resource->GetExports().Get();
    if (dict == nullptr) return nullptr;
    auto value = dict->Get(key);
    if (value.Get() == nullptr) {
        return nullptr;
    }
    return new alt::MValueConst(value);
}

int Resource_GetDependenciesSize(alt::IResource* resource) {
    return resource->GetDependencies().GetSize();
}

void Resource_GetDependencies(alt::IResource* resource, const char* dependencies[], int size) {

    if (resource->GetDependencies().GetSize() != size) return;
    for (uint64_t i = 0, length = resource->GetDependencies().GetSize(); i < length; i++) {
        dependencies[i] = resource->GetDependencies()[i].CStr();
    }
}

int Resource_GetDependantsSize(alt::IResource* resource) {
    return resource->GetDependants().GetSize();
}

void Resource_GetDependants(alt::IResource* resource, const char* dependencies[], int size) {

    if (resource->GetDependants().GetSize() != size) return;
    for (uint64_t i = 0, length = resource->GetDependants().GetSize(); i < length; i++) {
        dependencies[i] = resource->GetDependants()[i].CStr();
    }
}

void Resource_SetExport(alt::ICore* core, alt::IResource* resource, const char* key, alt::MValueConst* val) {
    alt::MValueDict dict = resource->GetExports();
    if (dict.Get() == nullptr) {
        dict = core->CreateMValueDict();
        resource->SetExports(dict);
    }
    dict->Set(key, val->Get()->Clone());
}

void Resource_SetExports(alt::ICore* core, alt::IResource* resource, alt::MValueConst* val[], const char* keys[], int size) {
    alt::MValueDict dict = core->CreateMValueDict();
    for (int i = 0; i < size; i++) {
        dict->Set(keys[i], val[i]->Get()->Clone());
    }
    resource->SetExports(dict);
}

void Resource_GetPath(alt::IResource* resource, const char*&text) {
    text = resource->GetPath().CStr();
}

void Resource_GetName(alt::IResource* resource, const char*&text) {
    text = resource->GetName().CStr();
}

void Resource_GetMain(alt::IResource* resource, const char*&text) {
    text = resource->GetMain().CStr();
}

void Resource_GetType(alt::IResource* resource, const char*&text) {
    text = resource->GetType().CStr();
}

bool Resource_IsStarted(alt::IResource* resource) {
    return resource->IsStarted();
}

void Resource_Start(alt::IResource* resource) {
    resource->GetImpl()->Start();
}

void Resource_Stop(alt::IResource* resource) {
    resource->GetImpl()->Stop();
}

alt::IResource::Impl* Resource_GetImpl(alt::IResource* resource) {
    return resource->GetImpl();
}

CSharpResourceImpl* Resource_GetCSharpImpl(alt::IResource* resource) {
    return (CSharpResourceImpl*) resource->GetImpl();
}