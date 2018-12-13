using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Mapbox.Mapboxsdk.Maps;
using Com.Mapbox.Mapboxsdk.Constants;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Camera;
using Android.Graphics;

namespace MvvmCross_Application1.Droid.Views
{
    [Activity(Label = "HomeView")]
    public class HomeView : BaseView,//MapboxMap.IOnMarkerClickListener,
        MapboxMap.IOnMapClickListener,
        MapboxMap.IOnScrollListener,
        ViewTreeObserver.IOnGlobalLayoutListener

    {
        MapView mapView;
        MapboxMap Map;
        private bool firstLoad = true;
        private Icon stoppedIcon;
        private ImageView mapSwitcherImg;
        IList<MarkerViewOptions> lstMarker = new List<MarkerViewOptions>();
        MarkerViewOptions markerViewOptions;
        Button btnZoomOut;
        Button btnInOut;
        IList<PolylineOptions> lstPolyline = new List<PolylineOptions>();
        PolylineOptions polylineOptions = new PolylineOptions();
        IList<LatLng> lstLatLng = new List<LatLng>() {new LatLng(28.6026, 77.3769), new LatLng(28.6026, 77.3769), new LatLng(28.628454, 77.376945), new LatLng(28.5996, 77.3736), new LatLng(28.5974, 77.3827)};

        protected override int LayoutResource => Resource.Layout.HomeView;
        public Icon StoppedIcon => this.stoppedIcon ?? (this.stoppedIcon = GetIcon(Resource.Drawable.map_50_60_chevron));
        protected override void OnCreate(Bundle bundle)
        {
            int i = 0;          
            //lstMarker.Add(new MarkerOptions().SetPosition(new LatLng(28.6026, 77.3769)).SetTitle("RSystem").SetSnippet("Well come to RSystem").SetIcon(StoppedIcon));
            //lstMarker.Add(new MarkerOptions().SetPosition(new LatLng(28.628454, 77.376945)).SetTitle("RSystem").SetSnippet("Well come to RSystem").SetIcon(StoppedIcon));
            //lstMarker.Add(new MarkerOptions().SetPosition(new LatLng(28.5996, 77.3736)).SetTitle("RSystem").SetSnippet("Well come to RSystem").SetIcon(StoppedIcon));
            //lstMarker.Add(new MarkerOptions().SetPosition(new LatLng(28.5974, 77.3827)).SetTitle("RSystem").SetSnippet("Well come to RSystem").SetIcon(StoppedIcon));
            base.OnCreate(bundle);
            btnZoomOut = (Button)this.FindViewById<Button>(Resource.Id.zoomOutButton);
            btnZoomOut.Click += BtnZoomOut_Click;

            btnInOut = this.FindViewById<Button>(Resource.Id.zoomInButton);
            btnInOut.Click += BtnInOut_Click;
        

            mapSwitcherImg = this.FindViewById<ImageView>(Resource.Id.mapSwitcher);
            mapSwitcherImg.Click += MapSwitcherImg_Click;

            Com.Mapbox.Mapboxsdk.Mapbox.GetInstance(this, "pk.eyJ1Ijoic2FjaGluLXNhaW5pIiwiYSI6ImNqbzAyNHd5YTE5eHQzcW8wa3ptOWhiaXkifQ.jYVRdAzrjIGfsWVt34CB7w");

            mapView = FindViewById<MapView>(Resource.Id.mapView);
            mapView.SetStyleUrl(Style.MapboxStreets);
            mapView.OnCreate(bundle);
            this.performOperatoinOnMap((map) =>
            {
                this.Map = map;
                var position = new CameraPosition.Builder()
                                    .Target(new LatLng(28.60831, 77.36114))
                                    .Zoom(8)
                                    .Build();
                Map.AddMarker(new MarkerOptions().SetPosition(new LatLng(28.6208, 77.3639)).SetTitle("RSystem").SetSnippet("Well come to RSystem"));
                this.Map.MoveCamera(CameraUpdateFactory.NewCameraPosition(position));

            });
            //  SupportActionBar.SetDisplayHomeAsUpEnabled(false);
        }

        private void MapSwitcherImg_Click(object sender, EventArgs e)
        {
            if (Map.StyleUrl == Style.MapboxStreets)
            {
                mapView.SetStyleUrl(Style.SatelliteStreets);
                mapSwitcherImg.SetImageResource(Resource.Drawable.standard_map);
            }
            else
            {
                this.mapView.SetStyleUrl(Style.MapboxStreets);
                mapSwitcherImg.SetImageResource(Resource.Drawable.satellite_map);
            }
        }

        private void BtnInOut_Click(object sender, EventArgs e)
        {
            this.Map.MoveCamera(CameraUpdateFactory.ZoomIn());
        }

        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            this.Map.MoveCamera(CameraUpdateFactory.ZoomOut());
        }

      

        public void performOperatoinOnMap(Action<MapboxMap> action)
        {
            mapView.GetMapAsync(new OnMapReadyCallback(action));

        }
        private Icon GetIcon(int id)
        {
            return IconFactory.GetInstance(this).FromResource(id);
        }

        protected override void OnStart()
        { int i =0;
            base.OnStart();
            // this.AttachEvents();
            this.mapView.OnStart();
            if (this.firstLoad)
            {
                this.firstLoad = false;
                this.performOperatoinOnMap((map) =>
                {
                    this.Map = map;
                    this.Map.UiSettings.RotateGesturesEnabled = false;
                    this.Map.SetOnMapClickListener(this);
                    //this.Map.SetOnMarkerClickListener(this);                  
                    this.Map.SetOnScrollListener(this);
                    this.ZoomToDefaultView();
                    var position = new CameraPosition.Builder()
                                        .Target(new LatLng(28.60831, 77.36114))
                                        .Zoom(8)
                                        .Build();


                    foreach (var item in lstLatLng)
                    {
                        markerViewOptions = new MarkerViewOptions();
                        markerViewOptions.InvokeIcon(StoppedIcon);
                        markerViewOptions.InvokePosition(item);
                        markerViewOptions.InvokeRotation((float)item.Latitude);
                        markerViewOptions.InvokeSnippet("Well come to RSystem");
                        markerViewOptions.InvokeTitle("RSystem");
                        Map.AddMarker(markerViewOptions);
                       // lstMarker.Add(markerViewOptions);

                        polylineOptions.Add(item);
                        polylineOptions.InvokeWidth(5);
                        if (i % 2 == 0)
                            polylineOptions.InvokeColor(Color.Red);
                        else
                            polylineOptions.InvokeColor(Color.Green);
                        // lstPolyline.Add(polylineOptions);
                      //  Map.AddPolyline(polylineOptions);
                         i++;

                    }
             
                   // Map.AddPolylines(lstPolyline);        

                    this.Map.MoveCamera(CameraUpdateFactory.NewCameraPosition(position));
                });
            }
        }
        private void ZoomToDefaultView()
        {
            // LiveViewModel viewModel = (LiveViewModel)this.ViewModel;
            this.Map.AnimateCamera(CameraUpdateFactory.ZoomTo(8));
            // this.UpdateMap(viewModel.Vehicles, animated: false);
        }
        protected override void OnResume()
        {
            base.OnResume();
            mapView.OnResume();
        }
        protected override void OnPause()
        {
            mapView.OnPause();
            base.OnPause();
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mapView.OnSaveInstanceState(outState);
        }
        protected override void OnStop()
        {
            base.OnStop();
            mapView.OnStop();
        }
        protected override void OnDestroy()
        {
            mapView.OnDestroy();
            base.OnDestroy();
        }
        public override void OnLowMemory()
        {
            base.OnLowMemory();
            mapView.OnLowMemory();
        }

        //public bool OnMarkerClick(Marker p0)
        //{
        //    return true;
        //}

        public void OnMapClick(LatLng p0)
        {

        }

        public void OnScroll()
        {

        }

     
    }
    class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
    {
        public Action<MapboxMap> MapReady { get; set; }
        public OnMapReadyCallback(Action<MapboxMap> MapReady)
        {
            this.MapReady = MapReady;
        }
        public void OnMapReady(MapboxMap p0)
        {
            MapReady?.Invoke(p0);
        }
    }
}